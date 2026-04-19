using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using Mlt.Runtime;
using Mlt.Semantics.Exceptions;
using Mlt.Semantics.Helpers;
using Mlt.VirtualMachine.Exceptions;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Semantics.Passes;

public class CheckTypesPass : AbstractPass
{
    private MainFunctionDeclaration? _currentFunction;

    public override void Visit(Program node)
    {
        base.Visit(node);

        if (node.MainFunction.ResultType != ValueType.Int)
        {
            throw new TypeErrorException("Function main must return type int");
        }

        if (!HasReturnInBlock(node.MainFunction.Body))
        {
            throw new TypeErrorException("Function main must contain a return statement");
        }
    }

    public override void Visit(MainFunctionDeclaration d)
    {
        _currentFunction = d;
        base.Visit(d);
        _currentFunction = null;
    }

    public override void Visit(ReturnStatement node)
    {
        if (node.Expression != null)
        {
            node.Expression.Accept(this);

            if (node.Expression.ResultType != Mlt.Runtime.ValueType.Int)
            {
                throw new ProgramAbortedException("Критическая ошибка: функция main должна возвращать целое число (Int).");
            }
        }
    }

    public override void Visit(PrintStatement node)
    {
        base.Visit(node);
    }

    private bool HasReturnInBlock(BlockStatement block)
    {
        foreach (AstNode node in block.Nodes)
        {
            if (node is ReturnStatement)
            {
                return true;
            }

            if (node is BlockStatement innerBlock && HasReturnInBlock(innerBlock))
            {
                return true;
            }
        }

        return false;
    }
}