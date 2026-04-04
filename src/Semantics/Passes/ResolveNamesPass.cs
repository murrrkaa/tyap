using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Semantics.Exceptions;
using PsTiger.Semantics.Helpers;
using PsTiger.Semantics.Symbols;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PsTiger.Semantics.Passes;

/// <summary>
/// Проход по AST, устанавливающий соответствие имён и символов (объявлений).
/// </summary>
public sealed class ResolveNamesPass : AbstractPass
{
    /// <summary>
    /// В таблицу символов складываются объявления.
    /// </summary>
    private SymbolsTable _symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        _symbols = globalSymbols;
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);

        e.Function = _symbols.GetFunctionDeclaration(e.Name);
    }

    public override void Visit(ScopeExpression e)
    {
        // Выполняем отложенный обход узлов объявлений для реализации взаимной рекурсии объявлений.
        DeclarationVisitQueue visitQueue = new(this);

        // Создаём дочернюю таблицу символов.
        _symbols = new SymbolsTable(_symbols);
        try
        {
            // Обходим объявления, при этом идущие подряд функции объявляем заранее.
            foreach (Declaration d in e.Declarations)
            {
                switch (d)
                {
                    case FunctionDeclaration f:
                        // Заранее объявляем эту функцию и добавляем в очередь обхода.
                        visitQueue.BeforeFunctionDeclaration();
                        _symbols.DeclareFunction(f);
                        visitQueue.Enqueue(d);
                        break;
                    default:
                        visitQueue.Flush();
                        d.Accept(this);
                        break;
                }
            }

            visitQueue.Flush();

            // Обходим последовательность выражений в данной области видимости.
            foreach (Expression nested in e.Expressions)
            {
                nested.Accept(this);
            }
        }
        finally
        {
            // Возвращаемся к прежней таблице символов.
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(VariableAccessExpression e)
    {
        base.Visit(e);

        e.Variable = _symbols.GetVariableDeclaration(e.Name);
    }

    public override void Visit(VariableDeclaration d)
    {
        d.InitialValue.Accept(this);

        d.ResolvedDeclaredType = d.DeclaredReturnTypeName != null
            ? ValueTypeUtil.Parse(d.DeclaredReturnTypeName)
            : ValueType.Void;

        _symbols.DeclareVariable(d);
    }

    public override void Visit(FunctionDeclaration d)
    {
        d.ResolvedReturnType = d.DeclaredReturnTypeName != null
            ? ValueTypeUtil.Parse(d.DeclaredReturnTypeName)
            : ValueType.Void;

        // Создаём дочернюю таблицу символов.
        _symbols = new SymbolsTable(_symbols);
        try
        {
            // Обходим поддерево функции.
            base.Visit(d);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(ParameterDeclaration d)
    {
        d.ResolvedType = ValueTypeUtil.Parse(d.TypeName); 
        _symbols.DeclareVariable(d);
    }

    public override void Visit(ForLoopExpression e)
    {
        // Создаём дочернюю таблицу символов.
        _symbols = new SymbolsTable(_symbols);
        try
        {
            base.Visit(e);
        }
        finally
        {
            // Возвращаемся к прежней таблице символов.
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(ForIteratorDeclaration d)
    {
        base.Visit(d);
        _symbols.DeclareVariable(d);
    }
}