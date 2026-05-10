using System.Collections.Generic;

using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using Mlt.Semantics.Exceptions;
using Mlt.Semantics.Helpers;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Semantics.Passes;

public class CheckTypesPass : AbstractPass
{
    public override void Visit(Program node)
    {
        node.MainFunction.Accept(this);
    }

    public override void Visit(MainFunctionDeclaration node)
    {
        base.Visit(node);

        bool hasReturn = HasReturnStatement(node.Body);
        if (!hasReturn)
        {
            throw new TypeErrorException("Функция 'main' должна содержать оператор return");
        }

        foreach (ReturnStatement ret in FindReturnStatements(node.Body))
        {
            if (ret.Expression == null)
            {
                throw new TypeErrorException("Функция 'main' должна возвращать значение типа int");
            }

            if (!ValueTypeUtil.AreCompatibleTypes(ValueType.Int, ret.Expression.ResultType))
            {
                throw new TypeErrorException($"Функция 'main' должна возвращать int, но возвращает {ret.Expression.ResultType}");
            }
        }
    }

    public override void Visit(VariableDeclaration node)
    {
        base.Visit(node);
        if (node.Initializer != null)
        {
            if (!ValueTypeUtil.AreCompatibleTypes(node.Type, node.Initializer.ResultType))
            {
                throw new TypeErrorException($"Cannot initialize variable of type {node.Type} with expression of type {node.Initializer.ResultType}");
            }
        }
    }

    public override void Visit(AssignmentExpression node)
    {
        base.Visit(node);
        if (!ValueTypeUtil.AreCompatibleTypes(node.Left.ResultType, node.Right.ResultType))
        {
            throw new TypeErrorException($"Cannot assign {node.Right.ResultType} to {node.Left.ResultType}");
        }

        node.ResultType = node.Left.ResultType;
    }

    public override void Visit(BinaryOperationExpression node)
    {
        base.Visit(node);
        if (!ValueTypeUtil.AreCompatibleTypes(node.Left.ResultType, node.Right.ResultType))
        {
            throw new TypeErrorException($"Type mismatch in binary operation: {node.Left.ResultType} and {node.Right.ResultType}");
        }

        node.ResultType = node.Left.ResultType;
    }

    public override void Visit(LiteralExpression node)
    {
        node.ResultType = node.Type;
    }

    public override void Visit(VariableAccessExpression node)
    {
    }

    private static bool HasReturnStatement(BlockStatement block)
    {
        foreach (AstNode node in block.Nodes)
        {
            if (node is ReturnStatement)
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<ReturnStatement> FindReturnStatements(BlockStatement block)
    {
        foreach (AstNode node in block.Nodes)
        {
            if (node is ReturnStatement ret)
            {
                yield return ret;
            }
        }
    }
}