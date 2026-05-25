using System;
using System.Collections.Generic;
using System.Linq;

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
                [new BuiltinFunctionParameter("value", ValueType.Any)],
                ValueType.Void),
            ["readInt"] = new BuiltinFunction(
                "readInt",
                [],
                ValueType.Int),
            ["readFloat"] = new BuiltinFunction(
                "readFloat",
                [],
                ValueType.Float),
            ["readString"] = new BuiltinFunction(
                "readString",
                [],
                ValueType.String),
            ["len"] = new BuiltinFunction(
                "len",
                [new BuiltinFunctionParameter("s", ValueType.String)],
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
                [new BuiltinFunctionParameter("x", ValueType.Any)],
                ValueType.String),
            ["parseInt"] = new BuiltinFunction(
                "parseInt",
                [new BuiltinFunctionParameter("s", ValueType.String)],
                ValueType.Int),
            ["toBool"] = new BuiltinFunction(
                "toBool",
                [new BuiltinFunctionParameter("x", ValueType.Int)],
                ValueType.Bool),
            ["toFloat"] = new BuiltinFunction(
                "toFloat",
                [new BuiltinFunctionParameter("x", ValueType.Int)],
                ValueType.Float),
        };

    private readonly Stack<Dictionary<string, (AbstractVariableDeclaration Declaration, bool IsMutable)>> _scopes = new();

    private readonly HashSet<string> _customFunctions = new();

    public override void Visit(Program node)
    {
        PushScope();

        foreach (Declaration decl in node.TopLevelStatements)
        {
            if (decl is FunctionDeclaration func)
            {
                _customFunctions.Add(func.Name);
            }
        }

        base.Visit(node);
        PopScope();
    }

    public override void Visit(FunctionDeclaration node)
    {
        PushScope();

        foreach (ParameterDeclaration param in node.Parameters.OfType<ParameterDeclaration>())
        {
            Declare(param.Name, param, isMutable: false);
        }

        node.Body.Accept(this);
        PopScope();
    }

    public override void Visit(MainFunctionDeclaration node)
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
        Declare(node.Name, node, isMutable: true);
    }

    public override void Visit(ConstantDeclaration node)
    {
        node.InitialValue?.Accept(this);
        Declare(node.Name, node, isMutable: false);
    }

    public override void Visit(AssignmentStatement node)
    {
        if (!TryResolve(node.VariableName, out _, out bool isMutable))
        {
            throw new Exception(
                $"Семантическая ошибка: использование необъявленной переменной '{node.VariableName}'.");
        }

        if (!isMutable)
        {
            throw new Exception(
                $"Семантическая ошибка: попытка изменения константы '{node.VariableName}'.");
        }

        node.Value.Accept(this);
    }

    public override void Visit(VariableAccessExpression node)
    {
        if (!TryResolve(node.Name, out AbstractVariableDeclaration? decl, out _))
        {
            throw new Exception(
                $"Семантическая ошибка: использование необъявленной переменной '{node.Name}'.");
        }

        node.Variable = decl!;
    }

    public override void Visit(FunctionCallExpression node)
    {
        base.Visit(node);

        if (_customFunctions.Contains(node.Name))
        {
            return;
        }

        if (BuiltinFunctions.TryGetValue(node.Name, out BuiltinFunction? builtin))
        {
            node.Function = builtin;
            return;
        }

        throw new Exception($"Семантическая ошибка: неизвестная функция '{node.Name}'.");
    }

    private void PushScope()
    {
        _scopes.Push(new Dictionary<string, (AbstractVariableDeclaration, bool)>());
    }

    private void PopScope()
    {
        _scopes.Pop();
    }

    private void Declare(string name, AbstractVariableDeclaration decl, bool isMutable)
    {
        if (_scopes.Count == 0)
        {
            PushScope();
        }

        if (_scopes.Peek().ContainsKey(name))
        {
            throw new Exception(
                $"Семантическая ошибка: переменная '{name}' уже объявлена в текущей области видимости.");
        }

        _scopes.Peek()[name] = (decl, isMutable);
    }

    private bool TryResolve(
        string name,
        out AbstractVariableDeclaration? decl,
        out bool isMutable)
    {
        foreach (Dictionary<string, (AbstractVariableDeclaration Declaration, bool IsMutable)> scope in _scopes)
        {
            if (scope.TryGetValue(
                name,
                out (AbstractVariableDeclaration Declaration, bool IsMutable) entry))
            {
                decl = entry.Declaration;
                isMutable = entry.IsMutable;
                return true;
            }
        }

        decl = null;
        isMutable = false;
        return false;
    }
}