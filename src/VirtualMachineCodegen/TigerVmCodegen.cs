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
    private readonly InstructionsBuilder _builder = new();
    private CodegenSymbolsTable? _symbolsTable;

    public List<Instruction> GenerateCode(Program program)
    {
        _symbolsTable = new CodegenSymbolsTable(null);

        BasicBlock mainBlock = _builder.CreateBasicBlock();
        _symbolsTable.AddFunctionEntry(program.MainFunction.Name, mainBlock);

        program.MainFunction.Accept(this);

        _builder.InsertPoint = mainBlock;
        _builder.Append(new Instruction(InstructionCode.Halt));

        return _builder.Finish();
    }

    public void Visit(LiteralExpression e)
    {
        _builder.Append(new Instruction(InstructionCode.Push, e.Value));
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

    public void Visit(FunctionDeclaration d)
    {
        BasicBlock functionBlock = _symbolsTable!.GetFunctionEntry(d.Name);
        BasicBlock previousBlock = _builder.InsertPoint;
        _builder.InsertPoint = functionBlock;

        try
        {
            d.Body.Accept(this);
        }
        finally
        {
            _builder.InsertPoint = previousBlock;
        }
    }

    public void Visit(Program e)
    {
    }
}