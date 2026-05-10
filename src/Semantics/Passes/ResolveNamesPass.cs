using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using System.Collections.Generic;
using System;

namespace Mlt.Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private readonly Dictionary<string, bool> _variableMutability = new();

    public override void Visit(VariableDeclaration node)
    {
        if (_variableMutability.ContainsKey(node.Name))
        {
            throw new Exception($"Семантическая ошибка: Переменная '{node.Name}' уже объявлена.");
        }

        base.Visit(node);

        _variableMutability.Add(node.Name, node.IsMutable);
    }

    public override void Visit(VariableAccessExpression node)
    {
        if (!_variableMutability.ContainsKey(node.Name))
        {
            throw new Exception($"Семантическая ошибка: Использование необъявленной переменной '{node.Name}'.");
        }

        base.Visit(node);
    }

    public override void Visit(AssignmentExpression node)
    {
        if (node.Left is VariableAccessExpression varAccess)
        {
            if (_variableMutability.TryGetValue(varAccess.Name, out bool isMutable) && !isMutable)
            {
                throw new Exception($"Семантическая ошибка: Попытка изменения константы '{varAccess.Name}'.");
            }
        }
        base.Visit(node);
    }
}