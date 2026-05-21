using System;
using System.Collections.Generic;
using System.Globalization;

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
    private readonly Dictionary<string, (ValueType ReturnType, List<ValueType> ParamTypes)> _functions = new();
    private ValueType _currentExpectedReturnType = ValueType.Void;

    private void PushScope() => _scopes.Push(new Dictionary<string, ValueType>());
    private void PopScope() => _scopes.Pop();

    private void AddVariable(string name, ValueType type)
    {
        if (_scopes.Count == 0) PushScope();
        _scopes.Peek()[name] = type;
    }

    private bool TryLookupVariable(string name, out ValueType type)
    {
        foreach (Dictionary<string, ValueType> scope in _scopes)
        {
            if (scope.TryGetValue(name, out ValueType foundType))
            {
                type = foundType;
                return true;
            }
        }
        type = ValueType.Void;
        return false;
    }

    public override void Visit(Program node)
    {
        PushScope();

        foreach (Declaration decl in node.TopLevelStatements)
        {
            if (decl is FunctionDeclaration func)
            {
                List<ValueType> paramTypes = new List<ValueType>();
                foreach (ParameterDeclaration p in func.Parameters)
                {
                    paramTypes.Add(p.ResolvedType);
                }

                _functions[func.Name] = (func.ResolvedReturnType, paramTypes);
            }
        }

        _functions["print"] = (ValueType.Void, new List<ValueType> { ValueType.Any });

        foreach (Declaration decl in node.TopLevelStatements)
        {
            decl.Accept(this);
        }

        node.MainFunction.Accept(this);

        PopScope();
    }

    public override void Visit(MainFunctionDeclaration node)
    {
        PushScope();
        _currentExpectedReturnType = ValueType.Int;

        node.Body.Accept(this);

        if (!HasReturnStatement(node.Body))
            throw new TypeErrorException("Функция 'main' должна содержать оператор return");

        foreach (ReturnStatement ret in FindReturnStatements(node.Body))
        {
            if (ret.Expression == null)
                throw new TypeErrorException("Функция 'main' должна возвращать значение типа int");

            if (!ValueTypeUtil.AreCompatibleTypes(ValueType.Int, ret.Expression.ResultType))
                throw new TypeErrorException(
                    $"Функция 'main' должна возвращать int, но возвращает {ret.Expression.ResultType}");
        }

        PopScope();
    }

    public override void Visit(FunctionDeclaration node)
    {
        PushScope();
        ValueType previousReturnType = _currentExpectedReturnType;

        _currentExpectedReturnType = node.ResolvedReturnType;

        foreach (ParameterDeclaration param in node.Parameters)
        {
            AddVariable(param.Name, param.ResolvedType);
        }

        node.Body.Accept(this);

        if (node.ResolvedReturnType != ValueType.Void && !HasReturnStatement(node.Body))
        {
            throw new TypeErrorException($"Функция '{node.Name}' не все пути к коду возвращают значение.");
        }

        foreach (ReturnStatement ret in FindReturnStatements(node.Body))
        {
            ValueType actualType = ret.Expression?.ResultType ?? ValueType.Void;

            if (!ValueTypeUtil.AreCompatibleTypes(node.ResolvedReturnType, actualType))
            {
                throw new TypeErrorException(
                    $"Функция '{node.Name}' должна возвращать {node.ResolvedReturnType}, но возвращает {actualType}");
            }
        }

        _currentExpectedReturnType = previousReturnType;
        PopScope();
    }

    public override void Visit(VariableDeclaration node)
    {
        if (node.InitialValue != null)
        {
            node.InitialValue.Accept(this);
            if (!ValueTypeUtil.AreCompatibleTypes(node.ResolvedType, node.InitialValue.ResultType))
                throw new TypeErrorException(
                    $"Cannot initialize variable of type {node.ResolvedType} " +
                    $"with expression of type {node.InitialValue.ResultType}");
        }

        AddVariable(node.Name, node.ResolvedType);
    }

    public override void Visit(ConstantDeclaration node)
    {
        if (node.InitialValue != null)
        {
            node.InitialValue.Accept(this);
            if (!ValueTypeUtil.AreCompatibleTypes(node.ResolvedType, node.InitialValue.ResultType))
                throw new TypeErrorException(
                    $"Cannot initialize constant of type {node.ResolvedType} " +
                    $"with expression of type {node.InitialValue.ResultType}");
        }

        AddVariable(node.Name, node.ResolvedType);
    }

    public override void Visit(AssignmentStatement node)
    {
        if (!TryLookupVariable(node.VariableName, out ValueType targetType))
            throw new TypeErrorException($"Variable '{node.VariableName}' is not declared");

        node.Value.Accept(this);

        if (!ValueTypeUtil.AreCompatibleTypes(targetType, node.Value.ResultType))
            throw new TypeErrorException(
                $"Cannot assign {node.Value.ResultType} to {targetType}");
    }

    public override void Visit(BinaryOperationExpression node)
    {
        node.Left.Accept(this);
        node.Right.Accept(this);

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

        if (!ValueTypeUtil.AreCompatibleTypes(node.Left.ResultType, node.Right.ResultType))
            throw new TypeErrorException(
                $"Type mismatch in binary operation: {node.Left.ResultType} and {node.Right.ResultType}");

        if (node.Left.ResultType == ValueType.String && node.Operation != BinaryOperation.Add)
            throw new TypeErrorException(
                $"Operator {node.Operation} is not supported for type string. Only '+' is allowed.");

        if (node.Left.ResultType == ValueType.Bool)
            throw new TypeErrorException($"Operator {node.Operation} is not supported for type bool.");

        node.ResultType = node.Left.ResultType;
    }

    public override void Visit(UnaryNotExpression node)
    {
        node.Operand.Accept(this);

        if (node.Operand.ResultType != ValueType.Bool)
            throw new TypeErrorException(
                $"Operator '!' requires bool, but got {node.Operand.ResultType}");

        node.ResultType = ValueType.Bool;
    }

    public override void Visit(FunctionCallExpression node)
    {
        foreach (Expression arg in node.Arguments)
        {
            arg.Accept(this);
        }

        (ValueType ReturnType, List<ValueType> ParamTypes) funcSignature;
        if (!_functions.TryGetValue(node.Name, out funcSignature))
            throw new TypeErrorException($"Function '{node.Name}' is not declared");

        bool isVariadic = funcSignature.ParamTypes.Count == 1 && funcSignature.ParamTypes[0] == ValueType.Any;

        if (!isVariadic && node.Arguments.Count != funcSignature.ParamTypes.Count)
            throw new TypeErrorException(
                $"Function '{node.Name}' expects {funcSignature.ParamTypes.Count} arguments, but got {node.Arguments.Count}");

        if (!isVariadic)
        {
            for (int i = 0; i < funcSignature.ParamTypes.Count; i++)
            {
                ValueType expected = funcSignature.ParamTypes[i];
                ValueType actual = node.Arguments[i].ResultType;

                if (expected != ValueType.Any && !ValueTypeUtil.AreCompatibleTypes(expected, actual))
                    throw new TypeErrorException(
                        $"Argument {i + 1} of '{node.Name}' must be {expected}, but got {actual}");
            }
        }

        node.ResultType = funcSignature.ReturnType;
    }

    public override void Visit(LiteralExpression node)
    {
        node.ResultType = node.Type;
    }

    public override void Visit(VariableAccessExpression node)
    {
        if (!TryLookupVariable(node.Name, out ValueType targetType))
            throw new TypeErrorException($"Variable '{node.Name}' is not declared");

        node.ResultType = targetType;
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