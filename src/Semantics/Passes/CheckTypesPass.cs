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
    private readonly Stack<Dictionary<string, ValueType>> _scopes = new();

    public override void Visit(Program node)
    {
        node.MainFunction.Accept(this);
    }

    public override void Visit(MainFunctionDeclaration node)
    {
        PushScope();
        base.Visit(node);
        PopScope();

        if (!HasReturnStatement(node.Body))
            throw new TypeErrorException("Функция 'main' должна содержать оператор return");

        foreach (ReturnStatement ret in FindReturnStatements(node.Body))
        {
            if (ret.Expression == null)
                throw new TypeErrorException(
                    "Функция 'main' должна возвращать значение типа int");

            if (!ValueTypeUtil.AreCompatibleTypes(ValueType.Int, ret.Expression.ResultType))
                throw new TypeErrorException(
                    $"Функция 'main' должна возвращать int, но возвращает {ret.Expression.ResultType}");
        }
    }

    public override void Visit(VariableDeclaration node)
    {
        node.InitialValue?.Accept(this);

        if (node.InitialValue != null &&
            !ValueTypeUtil.AreCompatibleTypes(node.ResolvedType, node.InitialValue.ResultType))
        {
            throw new TypeErrorException(
                $"Cannot initialize variable '{node.Name}' of type {node.ResolvedType} " +
                $"with expression of type {node.InitialValue.ResultType}");
        }

        DeclareVariable(node.Name, node.ResolvedType);
    }

    public override void Visit(ConstantDeclaration node)
    {
        node.InitialValue?.Accept(this);

        if (node.InitialValue != null &&
            !ValueTypeUtil.AreCompatibleTypes(node.ResolvedType, node.InitialValue.ResultType))
        {
            throw new TypeErrorException(
                $"Cannot initialize constant '{node.Name}' of type {node.ResolvedType} " +
                $"with expression of type {node.InitialValue.ResultType}");
        }

        DeclareVariable(node.Name, node.ResolvedType);
    }

    public override void Visit(AssignmentStatement node)
    {
        node.Value.Accept(this);

        if (!TryResolveType(node.VariableName, out ValueType? varType))
            throw new TypeErrorException(
                $"Variable '{node.VariableName}' is not declared");

        if (!ValueTypeUtil.AreCompatibleTypes(varType!, node.Value.ResultType))
            throw new TypeErrorException(
                $"Cannot assign {node.Value.ResultType} to {varType}");
    }

    public override void Visit(BinaryOperationExpression node)
    {
        base.Visit(node);

        // Сравнения → результат bool
        if (node.Operation is BinaryOperation.Equal or BinaryOperation.NotEqual
            or BinaryOperation.LessThan or BinaryOperation.LessThanOrEqual
            or BinaryOperation.GreaterThan or BinaryOperation.GreaterThanOrEqual)
        {
            if (!ValueTypeUtil.AreCompatibleTypes(node.Left.ResultType, node.Right.ResultType))
                throw new TypeErrorException(
                    $"Type mismatch in comparison: {node.Left.ResultType} and {node.Right.ResultType}");

            node.ResultType = ValueType.Bool;
            return;
        }

        // Логические and/or → только bool
        if (node.Operation is BinaryOperation.And or BinaryOperation.Or)
        {
            if (node.Left.ResultType != ValueType.Bool)
                throw new TypeErrorException(
                    $"Left operand of '{node.Operation}' must be bool, but got {node.Left.ResultType}");

            if (node.Right.ResultType != ValueType.Bool)
                throw new TypeErrorException(
                    $"Right operand of '{node.Operation}' must be bool, but got {node.Right.ResultType}");

            node.ResultType = ValueType.Bool;
            return;
        }

        // Арифметика: типы должны совпадать
        if (!ValueTypeUtil.AreCompatibleTypes(node.Left.ResultType, node.Right.ResultType))
            throw new TypeErrorException(
                $"Type mismatch in binary operation: {node.Left.ResultType} and {node.Right.ResultType}");

        // Для строк только +
        if (node.Left.ResultType == ValueType.String && node.Operation != BinaryOperation.Add)
            throw new TypeErrorException(
                $"Operator {node.Operation} is not supported for type string. " +
                $"Only '+' (concatenation) is allowed.");

        // Арифметика запрещена для bool
        if (node.Left.ResultType == ValueType.Bool)
            throw new TypeErrorException(
                $"Operator {node.Operation} is not supported for type bool.");

        node.ResultType = node.Left.ResultType;
    }

    public override void Visit(UnaryNotExpression node)
    {
        base.Visit(node);

        if (node.Operand.ResultType != ValueType.Bool)
            throw new TypeErrorException(
                $"Operator '!' requires bool, but got {node.Operand.ResultType}");

        node.ResultType = ValueType.Bool;
    }

    public override void Visit(FunctionCallExpression node)
    {
        base.Visit(node);

        BuiltinFunction func = (BuiltinFunction)node.Function;

        bool isVariadic = func.Parameters.Count == 1 &&
            ((BuiltinFunctionParameter)func.Parameters[0]).Type == ValueType.Any;

        if (!isVariadic && node.Arguments.Count != func.Parameters.Count)
            throw new TypeErrorException(
                $"Function '{node.Name}' expects {func.Parameters.Count} arguments, " +
                $"but got {node.Arguments.Count}");

        if (!isVariadic)
        {
            for (int i = 0; i < func.Parameters.Count; i++)
            {
                ValueType expected = ((BuiltinFunctionParameter)func.Parameters[i]).Type;
                ValueType actual = node.Arguments[i].ResultType;

                if (expected != ValueType.Any &&
                    !ValueTypeUtil.AreCompatibleTypes(expected, actual))
                    throw new TypeErrorException(
                        $"Argument {i + 1} of '{node.Name}' must be {expected}, but got {actual}");
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
        if (!TryResolveType(node.Name, out ValueType? type))
            throw new TypeErrorException($"Variable '{node.Name}' is not declared");

        node.ResultType = type!;
    }

    private void PushScope() => _scopes.Push(new Dictionary<string, ValueType>());
    private void PopScope() => _scopes.Pop();

    private void DeclareVariable(string name, ValueType type)
    {
        if (_scopes.Count > 0)
            _scopes.Peek()[name] = type;
    }

    private bool TryResolveType(string name, out ValueType? type)
    {
        foreach (Dictionary<string, ValueType> scope in _scopes)
        {
            if (scope.TryGetValue(name, out type))
                return true;
        }

        type = null;
        return false;
    }

    private static bool HasReturnStatement(BlockStatement block)
    {
        foreach (AstNode node in block.Nodes)
            if (node is ReturnStatement) return true;
        return false;
    }

    private static IEnumerable<ReturnStatement> FindReturnStatements(BlockStatement block)
    {
        foreach (AstNode node in block.Nodes)
            if (node is ReturnStatement ret) yield return ret;
    }
}