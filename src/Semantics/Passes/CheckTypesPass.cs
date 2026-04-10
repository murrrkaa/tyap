using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Runtime;
using PsTiger.Semantics.Exceptions;
using PsTiger.Semantics.Helpers;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Semantics.Passes;

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

    public override void Visit(FunctionDeclaration d)
    {
        _currentFunction = d;
        base.Visit(d);
        _currentFunction = null;
    }

    public override void Visit(ReturnStatement e)
    {
        base.Visit(e);

        if (_currentFunction != null)
        {
            if (e.Expression != null)
            {
                if (!ValueTypeUtil.AreCompatibleTypes(e.Expression.ResultType, _currentFunction.ResultType))
                {
                    throw new TypeErrorException(
                        $"Return type {e.Expression.ResultType} does not match function {_currentFunction.Name} return type {_currentFunction.ResultType}"
                    );
                }
            }
            else if (_currentFunction.ResultType != ValueType.Void)
            {
                throw new TypeErrorException(
                    $"Function {_currentFunction.Name} expects return value of type {_currentFunction.ResolvedReturnType}"
                );
            }
        }
    }

    public override void Visit(PrintStatement node)
    {
        base.Visit(node);
        foreach (Expression arg in node.Arguments)
        {
            if (arg.ResultType == ValueType.Void)
            {
                throw new TypeErrorException("Cannot print void expression");
            }
        }
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