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

        if (!HasReturnStatement(node.Body))
        {
            throw new TypeErrorException(
                "Функция 'main' должна содержать оператор return");
        }

        foreach (ReturnStatement ret in FindReturnStatements(node.Body))
        {
            if (ret.Expression == null)
            {
                throw new TypeErrorException(
                    "Функция 'main' должна возвращать значение типа int");
            }

            if (!ValueTypeUtil.AreCompatibleTypes(ValueType.Int, ret.Expression.ResultType))
            {
                throw new TypeErrorException(
                    $"Функция 'main' должна возвращать int, но возвращает {ret.Expression.ResultType}");
            }
        }
    }

    public override void Visit(VariableDeclaration node)
    {
        node.InitialValue?.Accept(this);

        if (node.InitialValue != null &&
            !ValueTypeUtil.AreCompatibleTypes(node.ResolvedType, node.InitialValue.ResultType))
        {
            throw new TypeErrorException(
                $"Невозможно инициализировать переменную '{node.Name}' типа {node.ResolvedType} " +
                $"значением типа {node.InitialValue.ResultType}");
        }
    }

    public override void Visit(ConstantDeclaration node)
    {
        node.InitialValue?.Accept(this);

        if (node.InitialValue != null &&
            !ValueTypeUtil.AreCompatibleTypes(node.ResolvedType, node.InitialValue.ResultType))
        {
            throw new TypeErrorException(
                $"Невозможно инициализировать константу '{node.Name}' типа {node.ResolvedType} " +
                $"значением типа {node.InitialValue.ResultType}");
        }
    }

    public override void Visit(BinaryOperationExpression node)
    {
        base.Visit(node);

        if (node.Operation is BinaryOperation.Equal or BinaryOperation.NotEqual
            or BinaryOperation.LessThan or BinaryOperation.LessThanOrEqual
            or BinaryOperation.GreaterThan or BinaryOperation.GreaterThanOrEqual)
        {
            if (!ValueTypeUtil.AreCompatibleTypes(node.Left.ResultType, node.Right.ResultType))
            {
                throw new TypeErrorException(
                    $"Несовместимые типы в сравнении: {node.Left.ResultType} и {node.Right.ResultType}");
            }

            node.ResultType = ValueType.Bool;
            return;
        }

        if (node.Operation is BinaryOperation.And or BinaryOperation.Or)
        {
            if (node.Left.ResultType != ValueType.Bool)
            {
                throw new TypeErrorException(
                    $"Левый операнд '{node.Operation}' должен быть bool, но получен {node.Left.ResultType}");
            }

            if (node.Right.ResultType != ValueType.Bool)
            {
                throw new TypeErrorException(
                    $"Правый операнд '{node.Operation}' должен быть bool, но получен {node.Right.ResultType}");
            }

            node.ResultType = ValueType.Bool;
            return;
        }

        if (!ValueTypeUtil.AreCompatibleTypes(node.Left.ResultType, node.Right.ResultType))
        {
            throw new TypeErrorException(
                $"Несовместимые типы в операции: {node.Left.ResultType} и {node.Right.ResultType}");
        }

        if (node.Left.ResultType == ValueType.String && node.Operation != BinaryOperation.Add)
        {
            throw new TypeErrorException(
                $"Оператор {node.Operation} не поддерживается для строк. Разрешён только '+'.");
        }

        if (node.Left.ResultType == ValueType.Bool)
        {
            throw new TypeErrorException(
                $"Оператор {node.Operation} не поддерживается для типа bool.");
        }

        node.ResultType = node.Left.ResultType;
    }

    public override void Visit(UnaryNotExpression node)
    {
        base.Visit(node);

        if (node.Operand.ResultType != ValueType.Bool)
        {
            throw new TypeErrorException(
                $"Оператор '!' требует bool, но получен {node.Operand.ResultType}");
        }

        node.ResultType = ValueType.Bool;
    }

    public override void Visit(FunctionCallExpression node)
    {
        base.Visit(node);

        BuiltinFunction func = (BuiltinFunction)node.Function;

        bool isVariadic = func.Parameters.Count == 1 &&
            ((BuiltinFunctionParameter)func.Parameters[0]).Type == ValueType.Any;

        if (!isVariadic && node.Arguments.Count != func.Parameters.Count)
        {
            throw new TypeErrorException(
                $"Функция '{node.Name}' ожидает {func.Parameters.Count} аргументов, " +
                $"но получено {node.Arguments.Count}");
        }

        if (!isVariadic)
        {
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                ValueType expected = ((BuiltinFunctionParameter)func.Parameters[i]).Type;
                ValueType actual = node.Arguments[i].ResultType;

                if (expected != ValueType.Any &&
                    !ValueTypeUtil.AreCompatibleTypes(expected, actual))
                {
                    throw new TypeErrorException(
                        $"Аргумент {i + 1} функции '{node.Name}' должен быть {expected}, но получен {actual}");
                }
            }
        }

        node.ResultType = func.ResultType;
    }

    public override void Visit(LiteralExpression node)
    {
        node.ResultType = node.Type;
    }

    public override void Visit(VariableAccessExpression node)
    {
        node.ResultType = node.Variable switch
        {
            VariableDeclaration v => v.ResolvedType,
            ConstantDeclaration c => c.ResolvedType,
            ParameterDeclaration p => p.ResolvedType,
            _ => throw new TypeErrorException(
                $"Неизвестный тип объявления для переменной '{node.Name}'"),
        };
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