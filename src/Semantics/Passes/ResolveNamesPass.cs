using System;
using System.Collections.Generic;

using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;

namespace Mlt.Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private readonly Stack<Dictionary<string, bool>> _scopes = new();

    public override void Visit(BlockStatement node)
    {
        PushScope();
        base.Visit(node);
        PopScope();
    }

    public override void Visit(VariableDeclaration node)
    {
        base.Visit(node);
        Declare(node.Name, node.IsMutable);
    }

    public override void Visit(VariableAccessExpression node)
    {
        if (!TryResolve(node.Name, out _))
        {
            throw new Exception($"Семантическая ошибка: Использование необъявленной переменной '{node.Name}'.");
        }

        base.Visit(node);
    }

    public override void Visit(AssignmentExpression node)
    {
        if (node.Left is VariableAccessExpression varAccess)
        {
            if (!TryResolve(varAccess.Name, out bool isMutable))
            {
                throw new Exception($"Семантическая ошибка: Использование необъявленной переменной '{varAccess.Name}'.");
            }

            if (!isMutable)
            {
                throw new Exception($"Семантическая ошибка: Попытка изменения константы '{varAccess.Name}'.");
            }
        }

        base.Visit(node);
    }

    private void PushScope() => _scopes.Push(new Dictionary<string, bool>());

    private void PopScope() => _scopes.Pop();

    private void Declare(string name, bool isMutable)
    {
        if (_scopes.Peek().ContainsKey(name))
        {
            throw new Exception($"Семантическая ошибка: Переменная '{name}' уже объявлена.");
        }

        _scopes.Peek()[name] = isMutable;
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