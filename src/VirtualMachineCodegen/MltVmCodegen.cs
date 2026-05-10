using System.Collections.Generic;

using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using Mlt.Runtime;
using Mlt.VirtualMachine.Builtins;
using Mlt.VirtualMachine.Instructions;

namespace Mlt.VirtualMachineCodegen;

public class MltVmCodegen : IAstVisitor
{
    private readonly List<Instruction> _instructions = new();

    public List<Instruction> GenerateCode(Program program)
    {
        program.Accept(this);
        _instructions.Add(new Instruction(InstructionCode.Halt));

        return _instructions;
    }

    public void Visit(Program node)
    {
        node.MainFunction.Accept(this);
    }

    public void Visit(MainFunctionDeclaration node)
    {
        node.Body.Accept(this);
    }

    public void Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
    }

    public void Visit(ReturnStatement node)
    {
        node.Expression?.Accept(this);
    }

    public void Visit(BlockStatement node)
    {
        foreach (AstNode nodeItem in node.Nodes)
        {
            nodeItem.Accept(this);
        }
    }

    public void Visit(LiteralExpression node)
    {
        _instructions.Add(new Instruction(InstructionCode.Push, node.Value));
    }

    public void Visit(VariableDeclaration node)
    {
        node.Initializer?.Accept(this);
        _instructions.Add(new Instruction(InstructionCode.DefineVar, node.Name));
    }

    public void Visit(VariableAccessExpression node)
    {
        _instructions.Add(new Instruction(InstructionCode.LoadVar, node.Name));
    }

    public void Visit(AssignmentExpression node)
    {
        node.Right.Accept(this);

        if (node.Left is VariableAccessExpression varAccess)
        {
            _instructions.Add(new Instruction(InstructionCode.StoreVar, varAccess.Name));
        }
    }

    public void Visit(BinaryOperationExpression node)
    {
        node.Left.Accept(this);
        node.Right.Accept(this);

        InstructionCode code = node.Operation switch
        {
            BinaryOperation.Add => InstructionCode.Add,
            BinaryOperation.Subtract => InstructionCode.Subtract,
            BinaryOperation.Multiply => InstructionCode.Multiply,
            BinaryOperation.Divide => InstructionCode.Divide,
            _ => throw new System.NotImplementedException($"Operation {node.Operation} is not supported")
        };

        _instructions.Add(new Instruction(code));
    }

    public void Visit(PrintStatement node)
    {
        foreach (Expression arg in node.Arguments)
        {
            arg.Accept(this);

            _instructions.Add(
                new Instruction(
                    InstructionCode.CallBuiltin,
                    (int)BuiltinFunctionCode.Print
                )
            );
        }
    }
}