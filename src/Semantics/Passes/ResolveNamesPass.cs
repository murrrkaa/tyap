using System;
using System.Collections.Generic;

using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private static readonly IReadOnlyDictionary<string, BuiltinFunction> BuiltinFunctions =
        new Dictionary<string, BuiltinFunction>
        {
            ["print"] = new BuiltinFunction(
                "print",
                [
                    new BuiltinFunctionParameter("value", ValueType.Any)
                ],
                ValueType.Void),

            ["readInt"] = new BuiltinFunction("readInt", [], ValueType.Int),

            ["readFloat"] = new BuiltinFunction("readFloat", [], ValueType.Float),

            ["readString"] = new BuiltinFunction("readString", [], ValueType.String),

            ["len"] = new BuiltinFunction(
                "len",
                [
                    new BuiltinFunctionParameter("s", ValueType.String)
                ],
                ValueType.Int),

            ["substring"] = new BuiltinFunction(
                "substring",
                [
                    new BuiltinFunctionParameter("s", ValueType.String),
                    new BuiltinFunctionParameter("start", ValueType.Int),
                    new BuiltinFunctionParameter("count", ValueType.Int),
                ],
                ValueType.String),

            ["toString"] = new BuiltinFunction(
                "toString",
                [
                    new BuiltinFunctionParameter("x", ValueType.Any)
                ],
                ValueType.String),

            ["parseInt"] = new BuiltinFunction(
                "parseInt",
                [
                    new BuiltinFunctionParameter("s", ValueType.String)
                ],
                ValueType.Int),

            ["toBool"] = new BuiltinFunction(
                "toBool",
                [
                    new BuiltinFunctionParameter("x", ValueType.Int)
                ],
                ValueType.Bool),

            ["toFloat"] = new BuiltinFunction(
                "toFloat",
                [
                    new BuiltinFunctionParameter("x", ValueType.Int)
                ],
                ValueType.Float),
        };

    private readonly Stack<Dictionary<string, bool>> _scopes = new();

    public override void Visit(MainFunctionDeclaration node)
    {
        PushScope();
        base.Visit(node);
        PopScope();
    }

    public override void Visit(FunctionDeclaration node)
    {
        PushScope();
        base.Visit(node);
        PopScope();
    }

    public override void Visit(BlockStatement node)
    {
        PushScope();
        base.Visit(node);
        PopScope();
    }

    public override void Visit(VariableDeclaration node)
    {
        node.InitialValue?.Accept(this);
        Declare(node.Name, isMutable: true);
    }

    public override void Visit(ConstantDeclaration node)
    {
        node.InitialValue?.Accept(this);
        Declare(node.Name, isMutable: false);
    }

    public override void Visit(AssignmentStatement node)
    {
        if (!TryResolve(node.VariableName, out bool isMutable))
        {
            throw new Exception(
                $"Семантическая ошибка: Использование необъявленной переменной '{node.VariableName}'.");
        }

        if (!isMutable)
        {
            throw new Exception(
                $"Семантическая ошибка: Попытка изменения константы '{node.VariableName}'.");
        }

        node.Value.Accept(this);
    }

    public override void Visit(VariableAccessExpression node)
    {
        if (!TryResolve(node.Name, out _))
        {
            throw new Exception(
                $"Семантическая ошибка: Использование необъявленной переменной '{node.Name}'.");
        }
    }

    public override void Visit(FunctionCallExpression node)
    {
        base.Visit(node);

        if (!BuiltinFunctions.TryGetValue(node.Name, out BuiltinFunction? builtin))
        {
            throw new Exception(
                $"Семантическая ошибка: Неизвестная функция '{node.Name}'.");
        }

        node.Function = builtin;
    }

    private void PushScope() => _scopes.Push(new Dictionary<string, bool>());

    private void PopScope() => _scopes.Pop();

    private void Declare(string name, bool isMutable)
    {
        if (_scopes.Count > 0 && _scopes.Peek().ContainsKey(name))
        {
            throw new Exception($"Семантическая ошибка: Переменная '{name}' уже объявлена.");
        }

        if (_scopes.Count > 0)
        {
            _scopes.Peek()[name] = isMutable;
        }
    }

    private bool TryResolve(string name, out bool isMutable)
    {
        foreach (Dictionary<string, bool> scope in _scopes)
        {
            if (scope.TryGetValue(name, out isMutable))
            {
                return true;
            }
        }

        isMutable = false;
        return false;
    }
}