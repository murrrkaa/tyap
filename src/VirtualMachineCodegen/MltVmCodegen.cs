using System;
using System.Collections.Generic;
using System.Linq;

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

    public void Visit(Program node) => node.MainFunction.Accept(this);

    public void Visit(MainFunctionDeclaration node) => node.Body.Accept(this);

    public void Visit(FunctionDeclaration node)
    {
    }

    public void Visit(BuiltinFunction node)
    {
    }

    public void Visit(BuiltinFunctionParameter node)
    {
    }

    public void Visit(ParameterDeclaration node)
    {
    }

    public void Visit(BlockStatement node)
    {
        foreach (AstNode nodeItem in node.Nodes)
        {
            nodeItem.Accept(this);
        }
    }

    public void Visit(ReturnStatement node) => node.Expression?.Accept(this);

    public void Visit(ExpressionStatement node) => node.Expression.Accept(this);

    public void Visit(AssignmentStatement node)
    {
        node.Value.Accept(this);
        _instructions.Add(new Instruction(InstructionCode.StoreVar, node.VariableName));
    }

    public void Visit(FunctionCallStatement node) => node.Call.Accept(this);

    public void Visit(PrintStatement node)
    {
        foreach (Expression arg in node.Arguments)
        {
            arg.Accept(this);
            _instructions.Add(new Instruction(
                InstructionCode.CallBuiltin,
                new Value((long)BuiltinFunctionCode.Print)));
        }
    }

    public void Visit(VariableDeclaration node)
    {
        node.InitialValue?.Accept(this);
        _instructions.Add(new Instruction(InstructionCode.DefineVar, node.Name));
    }

    public void Visit(ConstantDeclaration node)
    {
        node.InitialValue?.Accept(this);
        _instructions.Add(new Instruction(InstructionCode.DefineVar, node.Name));
    }

    public void Visit(LiteralExpression node)
    {
        _instructions.Add(new Instruction(InstructionCode.Push, node.Value));
    }

    public void Visit(VariableAccessExpression node)
    {
        _instructions.Add(new Instruction(InstructionCode.LoadVar, node.Name));
    }

    public void Visit(BinaryOperationExpression node)
    {
        if (node.Operation == BinaryOperation.And)
        {
            node.Left.Accept(this);
            int jumpIndex = _instructions.Count;
            _instructions.Add(new Instruction(InstructionCode.JumpIfFalse, 0));
            _instructions.Add(new Instruction(InstructionCode.Pop));
            node.Right.Accept(this);
            _instructions[jumpIndex] = new Instruction(
                InstructionCode.JumpIfFalse, _instructions.Count);
            return;
        }

        if (node.Operation == BinaryOperation.Or)
        {
            node.Left.Accept(this);
            int jumpIndex = _instructions.Count;
            _instructions.Add(new Instruction(InstructionCode.JumpIfTrue, 0));
            _instructions.Add(new Instruction(InstructionCode.Pop));
            node.Right.Accept(this);
            _instructions[jumpIndex] = new Instruction(
                InstructionCode.JumpIfTrue, _instructions.Count);
            return;
        }

        node.Left.Accept(this);
        node.Right.Accept(this);

        InstructionCode code = node.Operation switch
        {
            BinaryOperation.Add => InstructionCode.Add,
            BinaryOperation.Subtract => InstructionCode.Subtract,
            BinaryOperation.Multiply => InstructionCode.Multiply,
            BinaryOperation.Divide => InstructionCode.Divide,
            BinaryOperation.Equal => InstructionCode.Equal,
            BinaryOperation.NotEqual => InstructionCode.NotEqual,
            BinaryOperation.LessThan => InstructionCode.Less,
            BinaryOperation.LessThanOrEqual => InstructionCode.LessOrEqual,
            BinaryOperation.GreaterThan => InstructionCode.GreaterThan,
            BinaryOperation.GreaterThanOrEqual => InstructionCode.GreaterThanOrEqual,
            _ => throw new NotImplementedException($"Operation {node.Operation} is not supported"),
        };

        _instructions.Add(new Instruction(code));
    }

    public void Visit(UnaryNotExpression node)
    {
        node.Operand.Accept(this);
        _instructions.Add(new Instruction(InstructionCode.Not));
    }

    public void Visit(FunctionCallExpression node)
    {
        foreach (Expression arg in node.Arguments)
        {
            arg.Accept(this);
        }

        BuiltinFunctionCode code = node.Name switch
        {
            "print" => BuiltinFunctionCode.Print,
            "readInt" => BuiltinFunctionCode.ReadInt,
            "readFloat" => BuiltinFunctionCode.ReadFloat,
            "readString" => BuiltinFunctionCode.ReadString,
            "len" => BuiltinFunctionCode.Len,
            "substring" => BuiltinFunctionCode.Substring,
            "toString" => BuiltinFunctionCode.ToString,
            "parseInt" => BuiltinFunctionCode.ParseInt,
            "toBool" => BuiltinFunctionCode.ToBool,
            "toFloat" => BuiltinFunctionCode.ToFloat,
            _ => throw new NotImplementedException($"Builtin '{node.Name}' is not supported"),
        };

        _instructions.Add(new Instruction(
            InstructionCode.CallBuiltin,
            new Value((long)code)));
    }
}