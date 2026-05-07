using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using System.Collections.Generic;
using System;

namespace Mlt.Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private readonly HashSet<string> _declaredVariables = new();

    public override void Visit(VariableDeclaration node)
    {
        if (_declaredVariables.Contains(node.Name))
        {
            throw new Exception($"Семантическая ошибка: Переменная '{node.Name}' уже объявлена.");
        }

        base.Visit(node);

        _declaredVariables.Add(node.Name);
    }

    public override void Visit(VariableAccessExpression node)
    {
        if (!_declaredVariables.Contains(node.Name))
        {
            throw new Exception($"Семантическая ошибка: Использование необъявленной переменной '{node.Name}'.");
        }

        base.Visit(node);
    }

    public override void Visit(AssignmentExpression node)
    {
        base.Visit(node);
    }
}