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