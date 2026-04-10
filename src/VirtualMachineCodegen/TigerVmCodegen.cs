using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Runtime;
using PsTiger.VirtualMachine.Builtins;
using PsTiger.VirtualMachine.Instructions;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.VirtualMachineCodegen;

public class TigerVmCodegen : IAstVisitor
{
    private static readonly IReadOnlyDictionary<string, BuiltinFunctionCode> BuiltinFunctionsMap =
        new Dictionary<string, BuiltinFunctionCode>
        {
            { "print", BuiltinFunctionCode.Print },
            { "readInt", BuiltinFunctionCode.ReadInt },
            { "readFloat", BuiltinFunctionCode.ReadFloat },
            { "readString", BuiltinFunctionCode.ReadString },
            { "len", BuiltinFunctionCode.Len },
            { "substring", BuiltinFunctionCode.Substring },
            { "toString", BuiltinFunctionCode.ToString },
            { "parseInt", BuiltinFunctionCode.ParseInt },
            { "toBool", BuiltinFunctionCode.ToBool },
            { "toFloat", BuiltinFunctionCode.ToFloat },
        };

    private readonly InstructionsBuilder _builder = new();
    private CodegenSymbolsTable? _symbolsTable;
    private readonly Stack<BasicBlock> _loopEndBlocks = new();

    public List<Instruction> GenerateCode(Program program)
    {
        _symbolsTable = new CodegenSymbolsTable(null);

        foreach (Declaration decl in program.TopLevelStatements)
        {
            if (decl is FunctionDeclaration func)
            {
                BasicBlock block = _builder.CreateBasicBlock();
                _symbolsTable.AddFunctionEntry(func.Name, block);
            }
        }

        BasicBlock mainBlock = _builder.CreateBasicBlock();
        _symbolsTable.AddFunctionEntry(program.MainFunction.Name, mainBlock);

        foreach (Declaration decl in program.TopLevelStatements)
        {
            decl.Accept(this);
        }

        program.MainFunction.Accept(this);

        _builder.InsertPoint = mainBlock;
        _builder.Append(new Instruction(InstructionCode.Halt));

        return _builder.Finish();
    }

    public void Visit(LiteralExpression e)
    {
        _builder.Append(new Instruction(InstructionCode.Push, e.Value));
    }

    public void Visit(VariableAccessExpression e)
    {
        _builder.Append(new Instruction(InstructionCode.LoadVar, e.Variable.Name));
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);

        InstructionCode code = e.Operation switch
        {
            BinaryOperation.Add => InstructionCode.Add,
            BinaryOperation.Subtract => InstructionCode.Subtract,
            BinaryOperation.Multiply => InstructionCode.Multiply,
            BinaryOperation.Divide => InstructionCode.Divide,
            BinaryOperation.And => InstructionCode.And,
            BinaryOperation.Or => InstructionCode.Or,
            BinaryOperation.Equal => InstructionCode.Equal,
            BinaryOperation.NotEqual => InstructionCode.NotEqual,
            BinaryOperation.LessThan => InstructionCode.Less,
            BinaryOperation.LessThanOrEqual => InstructionCode.LessOrEqual,
            BinaryOperation.GreaterThan => InstructionCode.GreaterThan,
            BinaryOperation.GreaterThanOrEqual => InstructionCode.GreaterThanOrEqual,
            _ => throw new NotImplementedException($"Unsupported binary operation: {e.Operation}"),
        };
        _builder.Append(new Instruction(code));
    }

    public void Visit(UnaryNotExpression e)
    {
        e.Operand.Accept(this);
        _builder.Append(new Instruction(InstructionCode.Not));
    }

    public void Visit(FunctionCallExpression e)
    {
        foreach (Expression arg in e.Arguments)
        {
            arg.Accept(this);
        }

        if (e.Function is BuiltinFunction builtin)
        {
            if (BuiltinFunctionsMap.TryGetValue(builtin.Name, out BuiltinFunctionCode code))
            {
                _builder.Append(new Instruction(InstructionCode.CallBuiltin, (int)code));
            }
            else
            {
                throw new NotImplementedException($"Unknown builtin: {builtin.Name}");
            }
        }
        else if (e.Function is FunctionDeclaration func)
        {
            BasicBlock block = _symbolsTable!.GetFunctionEntry(func.Name);
            _builder.AppendJump(InstructionCode.Call, block);
        }
    }

    public void Visit(AssignmentStatement e)
    {
        e.Value.Accept(this);
        _builder.Append(new Instruction(InstructionCode.StoreVar, e.VariableName));
    }

    public void Visit(IfStatement e)
    {
        BasicBlock elseBlock = _builder.CreateBasicBlock();
        BasicBlock finalBlock = _builder.CreateBasicBlock();

        e.Condition.Accept(this);
        _builder.AppendJump(InstructionCode.JumpIfFalse, elseBlock);

        e.ThenBranch.Accept(this);
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        _builder.InsertPoint = elseBlock;
        if (e.ElseBranch != null)
        {
            e.ElseBranch.Accept(this);
        }

        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        _builder.InsertPoint = finalBlock;
    }

    public void Visit(WhileStatement e)
    {
        BasicBlock loopStart = _builder.CreateBasicBlock();
        BasicBlock loopEnd = _builder.CreateBasicBlock();
        _loopEndBlocks.Push(loopEnd);

        _builder.AppendJump(InstructionCode.Jump, loopStart);
        _builder.InsertPoint = loopStart;

        e.Condition.Accept(this);
        _builder.AppendJump(InstructionCode.JumpIfFalse, loopEnd);

        e.Body.Accept(this);
        _builder.AppendJump(InstructionCode.Jump, loopStart);

        _loopEndBlocks.Pop();
        _builder.InsertPoint = loopEnd;
    }

    public void Visit(ForStatement e)
    {
        BasicBlock loopStart = _builder.CreateBasicBlock();
        BasicBlock loopEnd = _builder.CreateBasicBlock();
        _loopEndBlocks.Push(loopEnd);

        PushLexicalScope();

        e.Init.Accept(this);

        _builder.AppendJump(InstructionCode.Jump, loopStart);
        _builder.InsertPoint = loopStart;

        e.Condition.Accept(this);
        _builder.AppendJump(InstructionCode.JumpIfFalse, loopEnd);

        e.Body.Accept(this);

        e.Step.Accept(this);
        _builder.AppendJump(InstructionCode.Jump, loopStart);

        _loopEndBlocks.Pop();
        _builder.InsertPoint = loopEnd;
        PopLexicalScope();
    }

    public void Visit(BreakStatement e)
    {
        BasicBlock loopEnd = _loopEndBlocks.Peek();
        _builder.AppendJump(InstructionCode.Jump, loopEnd);
    }

    public void Visit(ContinueStatement e)
    {
        BasicBlock loopEnd = _loopEndBlocks.Peek();
        _builder.AppendJump(InstructionCode.Jump, loopEnd);
    }

    public void Visit(ReturnStatement e)
    {
        if (e.Expression != null)
        {
            e.Expression.Accept(this);
        }

        _builder.Append(new Instruction(InstructionCode.Return));
    }

    public void Visit(PrintStatement e)
    {
        foreach (Expression arg in e.Arguments)
        {
            arg.Accept(this);
            _builder.Append(new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print));
        }
    }

    public void Visit(FunctionCallStatement e)
    {
        e.Call.Accept(this);
        _builder.Append(new Instruction(InstructionCode.Pop));
    }

    public void Visit(BlockStatement e)
    {
        foreach (AstNode node in e.Nodes)
        {
            if (node is Statement stmt)
            {
                stmt.Accept(this);
            }
        }
    }

    public void Visit(VariableDeclaration d)
    {
        d.InitialValue.Accept(this);
        _builder.Append(new Instruction(InstructionCode.DefineVar, d.Name));
    }

    public void Visit(ConstantDeclaration d)
    {
        d.InitialValue.Accept(this);
        _builder.Append(new Instruction(InstructionCode.DefineVar, d.Name));
    }

    public void Visit(FunctionDeclaration d)
    {
        BasicBlock functionBlock = _symbolsTable!.GetFunctionEntry(d.Name);
        BasicBlock previousBlock = _builder.InsertPoint;
        _builder.InsertPoint = functionBlock;

        try
        {
            PushLexicalScope();

            foreach (ParameterDeclaration param in d.Parameters)
            {
                _builder.Append(new Instruction(InstructionCode.DefineVar, param.Name));
            }

            d.Body.Accept(this);

            PopLexicalScope();
        }
        finally
        {
            _builder.InsertPoint = previousBlock;
        }
    }

    public void Visit(ParameterDeclaration d)
    {
    }

    public void Visit(Program e)
    {
    }

    public void Visit(BuiltinFunction e)
    {
    }

    public void Visit(BuiltinFunctionParameter e)
    {
    }

    private bool IsJump(InstructionCode code)
    {
        return code is InstructionCode.Jump or InstructionCode.JumpIfTrue or InstructionCode.JumpIfFalse or InstructionCode.Call;
    }

    private void PushLexicalScope()
    {
        int parentDepth = _symbolsTable?.Depth ?? 0;
        _symbolsTable = new CodegenSymbolsTable(_symbolsTable);
        _builder.Append(new Instruction(InstructionCode.PushVars, parentDepth));
    }

    private void PopLexicalScope()
    {
        _builder.Append(new Instruction(InstructionCode.PopVars));
        _symbolsTable = _symbolsTable!.Parent;
    }
}