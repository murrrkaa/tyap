using System.Collections.Generic;

using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using Mlt.Semantics.Exceptions;
using Mlt.Semantics.Helpers;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Semantics.Passes;

public sealed class CheckTypesPass : AbstractPass
{
    private FunctionDeclaration? _currentFunction;

    public override void Visit(Program node)
    {
        base.Visit(node);
    }

    public override void Visit(MainFunctionDeclaration node)
    {
        base.Visit(node);

        if (!HasReturnStatement(node.Body))
        {
            throw new TypeErrorException(
                "Функция 'main' должна содержать оператор return.");
        }

        foreach (ReturnStatement ret in FindReturnStatements(node.Body))
        {
            if (ret.Expression == null)
            {
                throw new TypeErrorException(
                    "Функция 'main' должна возвращать значение типа int.");
            }

            if (!ValueTypeUtil.AreCompatibleTypes(
                    ValueType.Int,
                    ret.Expression.ResultType))
            {
                throw new TypeErrorException(
                    $"Функция 'main' должна возвращать int, а возвращает {ret.Expression.ResultType}.");
            }
        }
    }

    public override void Visit(FunctionDeclaration node)
    {
        FunctionDeclaration? previous = _currentFunction;
        _currentFunction = node;

        base.Visit(node);

        if (!HasReturnStatement(node.Body))
        {
            throw new TypeErrorException(
                $"Функция '{node.Name}' должна содержать оператор return.");
        }

        foreach (ReturnStatement ret in FindReturnStatements(node.Body))
        {
            if (node.ResolvedReturnType == ValueType.Void)
            {
                if (ret.Expression != null)
                {
                    throw new TypeErrorException(
                        $"Функция '{node.Name}' имеет тип void и не должна возвращать значение.");
                }
            }
            else
            {
                if (ret.Expression == null)
                {
                    throw new TypeErrorException(
                        $"Функция '{node.Name}' должна возвращать значение типа {node.ResolvedReturnType}.");
                }

                if (!ValueTypeUtil.AreCompatibleTypes(
                        node.ResolvedReturnType,
                        ret.Expression.ResultType))
                {
                    throw new TypeErrorException(
                        $"Функция '{node.Name}' должна возвращать {node.ResolvedReturnType}, а возвращает {ret.Expression.ResultType}.");
                }
            }
        }

        _currentFunction = previous;
    }

    public override void Visit(VariableDeclaration node)
    {
        node.InitialValue?.Accept(this);

        if (node.InitialValue != null &&
            !ValueTypeUtil.AreCompatibleTypes(
                node.ResolvedType,
                node.InitialValue.ResultType))
        {
            throw new TypeErrorException(
                $"Невозможно присвоить значение типа {node.InitialValue.ResultType} переменной '{node.Name}' типа {node.ResolvedType}.");
        }
    }

    public override void Visit(ConstantDeclaration node)
    {
        node.InitialValue?.Accept(this);

        if (node.InitialValue != null &&
            !ValueTypeUtil.AreCompatibleTypes(
                node.ResolvedType,
                node.InitialValue.ResultType))
        {
            throw new TypeErrorException(
                $"Невозможно присвоить значение типа {node.InitialValue.ResultType} константе '{node.Name}' типа {node.ResolvedType}.");
        }
    }

    public override void Visit(BinaryOperationExpression node)
    {
        base.Visit(node);

        if (node.Operation is BinaryOperation.And or BinaryOperation.Or)
        {
            if (node.Left.ResultType != ValueType.Bool)
            {
                throw new TypeErrorException(
                    $"Левый операнд '{node.Operation}' должен иметь тип bool.");
            }

            if (node.Right.ResultType != ValueType.Bool)
            {
                throw new TypeErrorException(
                    $"Правый операнд '{node.Operation}' должен иметь тип bool.");
            }

            return;
        }

        if (node.Operation is BinaryOperation.Equal
            or BinaryOperation.NotEqual
            or BinaryOperation.LessThan
            or BinaryOperation.LessThanOrEqual
            or BinaryOperation.GreaterThan
            or BinaryOperation.GreaterThanOrEqual)
        {
            if (!ValueTypeUtil.AreCompatibleTypes(
                    node.Left.ResultType,
                    node.Right.ResultType))
            {
                throw new TypeErrorException(
                    $"Несовместимые типы: {node.Left.ResultType} и {node.Right.ResultType}.");
            }

            return;
        }

        if (!ValueTypeUtil.AreCompatibleTypes(
                node.Left.ResultType,
                node.Right.ResultType))
        {
            throw new TypeErrorException(
                $"Несовместимые типы: {node.Left.ResultType} и {node.Right.ResultType}.");
        }

        if (node.Left.ResultType == ValueType.String &&
            node.Operation != BinaryOperation.Add)
        {
            throw new TypeErrorException(
                "Для строк разрешена только операция '+'.");
        }

        if (node.Left.ResultType == ValueType.Bool)
        {
            throw new TypeErrorException(
                $"Операция '{node.Operation}' не поддерживается для типа bool.");
        }
    }

    public override void Visit(UnaryNotExpression node)
    {
        base.Visit(node);

        if (node.Operand.ResultType != ValueType.Bool)
        {
            throw new TypeErrorException(
                $"Оператор '!' требует тип bool, получен {node.Operand.ResultType}.");
        }
    }

    public override void Visit(FunctionCallExpression node)
    {
        base.Visit(node);

        if (node.Function is BuiltinFunction builtin)
        {
            CheckBuiltinCall(node, builtin);
            return;
        }

        if (node.Function is FunctionDeclaration func)
        {
            if (node.Arguments.Count != func.Parameters.Count)
            {
                throw new TypeErrorException(
                    $"Функция '{func.Name}' ожидает {func.Parameters.Count} аргументов, получено {node.Arguments.Count}.");
            }

            for (int i = 0; i < func.Parameters.Count; i++)
            {
                ParameterDeclaration param =
                    (ParameterDeclaration)func.Parameters[i];

                if (!ValueTypeUtil.AreCompatibleTypes(
                        param.ResolvedType,
                        node.Arguments[i].ResultType))
                {
                    throw new TypeErrorException(
                        $"Аргумент {i + 1} функции '{func.Name}' должен иметь тип {param.ResolvedType}, получен {node.Arguments[i].ResultType}.");
                }
            }
        }
    }

    private static void CheckBuiltinCall(
        FunctionCallExpression node,
        BuiltinFunction func)
    {
        bool isVariadic =
            func.Parameters.Count == 1 &&
            ((BuiltinFunctionParameter)func.Parameters[0]).Type == ValueType.Any;

        if (!isVariadic &&
            node.Arguments.Count != func.Parameters.Count)
        {
            throw new TypeErrorException(
                $"Функция '{func.Name}' ожидает {func.Parameters.Count} аргументов, получено {node.Arguments.Count}.");
        }

        if (isVariadic)
        {
            return;
        }

        for (int i = 0; i < func.Parameters.Count; i++)
        {
            ValueType expected =
                ((BuiltinFunctionParameter)func.Parameters[i]).Type;

            ValueType actual =
                node.Arguments[i].ResultType;

            if (expected != ValueType.Any &&
                !ValueTypeUtil.AreCompatibleTypes(expected, actual))
            {
                throw new TypeErrorException(
                    $"Аргумент {i + 1} функции '{func.Name}' должен иметь тип {expected}, получен {actual}.");
            }
        }
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

    private static IEnumerable<ReturnStatement> FindReturnStatements(
        BlockStatement block)
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