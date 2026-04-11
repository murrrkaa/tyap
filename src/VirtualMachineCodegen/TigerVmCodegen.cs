using System.Collections.Generic;

using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Runtime;
using PsTiger.VirtualMachine.Builtins;
using PsTiger.VirtualMachine.Instructions;

namespace PsTiger.VirtualMachineCodegen;

public class TigerVmCodegen : IAstVisitor
{
    private readonly List<Instruction> _instructions = new();

    public List<Instruction> GenerateCode(Program program)
    {
        program.Accept(this);
        _instructions.Add(new Instruction(InstructionCode.Halt));

        return _instructions;
    }

    public void Visit(Program e)
    {
        e.MainFunction.Accept(this);
    }

    public void Visit(MainFunctionDeclaration d)
    {
        d.Body.Accept(this);
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

    public void Visit(LiteralExpression e)
    {
        _instructions.Add(new Instruction(InstructionCode.Push, e.Value));
    }

    public void Visit(PrintStatement e)
    {
        foreach (Expression arg in e.Arguments)
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

    public void Visit(ReturnStatement e)
    {
        e.Expression!.Accept(this);
    }
}