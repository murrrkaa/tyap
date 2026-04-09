using System.Diagnostics;

using PsTiger.Ast.Declarations;
using PsTiger.Semantics.Exceptions;

namespace PsTiger.Semantics.Symbols;

/// <summary>
/// Таблица символов, основанная на лексических областях видимости (областях действия) символов в коде.
/// </summary>
public sealed class SymbolsTable
{
    private readonly SymbolsTable? _parent;
    private readonly Dictionary<string, Declaration> _symbols;

    public SymbolsTable(SymbolsTable? parent)
    {
        _parent = parent;
        _symbols = [];
    }

    public SymbolsTable? Parent => _parent;

    public AbstractVariableDeclaration GetVariableDeclaration(string name)
    {
        Declaration? declaration = FindDeclaration(name);
        return declaration switch
        {
            AbstractVariableDeclaration variable => variable,
            AbstractFunctionDeclaration _ => throw new InvalidSymbolException(name, "variable", "function"),
            null => throw UnknownSymbolException.UndefinedVariableOrFunction(name),
            _ => throw new UnreachableException(),
        };
    }

    public AbstractFunctionDeclaration GetFunctionDeclaration(string name)
    {
        Declaration? declaration = FindDeclaration(name);
        return declaration switch
        {
            AbstractFunctionDeclaration function => function,
            AbstractVariableDeclaration _ => throw new InvalidSymbolException(name, "function", "variable"),
            null => throw UnknownSymbolException.UndefinedVariableOrFunction(name),
            _ => throw new UnreachableException(),
        };
    }

    public void DeclareVariable(AbstractVariableDeclaration symbol)
    {
        if (!_symbols.TryAdd(symbol.Name, symbol))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(symbol.Name);
        }
    }

    public void DeclareFunction(AbstractFunctionDeclaration symbol)
    {
        if (!_symbols.TryAdd(symbol.Name, symbol))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(symbol.Name);
        }
    }

    public void DeclareParameter(AbstractVariableDeclaration symbol)
    {
        if (!_symbols.TryAdd(symbol.Name, symbol))
        {
            throw DuplicateSymbolException.ParameterHidesLocalVariable(symbol.Name);
        }
    }

    private Declaration? FindDeclaration(string name)
    {
        if (_symbols.TryGetValue(name, out Declaration? declaration))
        {
            return declaration;
        }
        return _parent?.FindDeclaration(name);
    }
}