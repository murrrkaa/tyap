using System;
using System.Collections.Generic;

using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;

namespace Mlt.Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private readonly Stack<Dictionary<string, bool>> _scopes = new();
    private readonly HashSet<string> _customFunctions = new();

    private void PushScope() => _scopes.Push(new Dictionary<string, bool>());
    private void PopScope() => _scopes.Pop();

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

        foreach (ParameterDeclaration param in node.Parameters)
        {
            Declare(param.Name, isMutable: false);
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
                $"Семантическая ошибка: Попытка изменения константы или параметра '{node.VariableName}'.");
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

        base.Visit(node);
    }

    public override void Visit(FunctionCallExpression node)
    {
        foreach (Expression arg in node.Arguments)
        {
            arg.Accept(this);
        }

        if (_customFunctions.Contains(node.Name))
        {
            return;
        }

        if (node.Name == "print")
        {
            return;
        }

        throw new Exception($"Семантическая ошибка: Неизвестная функция '{node.Name}'.");
    }

    private void Declare(string name, bool isMutable)
    {
        if (_scopes.Count == 0) PushScope();

        if (_scopes.Peek().ContainsKey(name))
        {
            throw new Exception(
                $"Семантическая ошибка: Переменная или параметр '{name}' уже объявлена в текущей области видимости.");
        }

        _scopes.Peek()[name] = isMutable;
    }

    private bool TryResolve(string name, out bool isMutable)
    {
        foreach (Dictionary<string, bool> scope in _scopes)
        {
            if (scope.TryGetValue(name, out isMutable))
                return true;
        }

        isMutable = false;
        return false;
    }
}