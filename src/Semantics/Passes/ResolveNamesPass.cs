using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Semantics.Exceptions;
using PsTiger.Semantics.Symbols;

namespace PsTiger.Semantics.Passes;

/// <summary>
/// Проход по AST, устанавливающий соответствие имён и символов (объявлений).
/// </summary>
public sealed class ResolveNamesPass : AbstractPass
{
    private SymbolsTable _symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        _symbols = globalSymbols;
    }

    public override void Visit(Program p)
    {
        foreach (Declaration declaration in p.TopLevelStatements)
        {
            if (declaration is FunctionDeclaration func)
            {
                _symbols.DeclareFunction(func);
            }
        }

        foreach (Declaration declaration in p.TopLevelStatements)
        {
            declaration.Accept(this);
        }

        p.MainFunction.Accept(this);
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        e.Function = _symbols.GetFunctionDeclaration(e.Name);
    }

    public override void Visit(VariableAccessExpression e)
    {
        base.Visit(e);
        e.Variable = _symbols.GetVariableDeclaration(e.Name);
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        _symbols.DeclareVariable(d);
    }

    public override void Visit(ConstantDeclaration d)
    {
        base.Visit(d);
        _symbols.DeclareVariable(d);
    }

    public override void Visit(FunctionDeclaration d)
    {
        _symbols = new SymbolsTable(_symbols);
        try
        {
            foreach (ParameterDeclaration parameter in d.Parameters)
            {
                _symbols.DeclareParameter(parameter);
            }
            base.Visit(d);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);
    }

    public override void Visit(BlockStatement s)
    {
        _symbols = new SymbolsTable(_symbols);
        try
        {
            base.Visit(s);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(ForStatement s)
    {
        _symbols = new SymbolsTable(_symbols);
        try
        {
            base.Visit(s);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }
}