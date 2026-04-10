using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Semantics.Passes;
using PsTiger.Semantics.Symbols;

namespace PsTiger.Semantics;

public class SemanticsChecker
{
    private readonly AbstractPass[] _passes;

    public SemanticsChecker()
    {
        SymbolsTable globalSymbols = new(parent: null);

        foreach (BuiltinFunction function in Builtins.Functions)
        {
            globalSymbols.DeclareFunction(function);
        }

        _passes =
        [
            new ResolveNamesPass(globalSymbols),
            new CheckContextSensitiveRulesPass(),
            new ResolveTypesPass(),
            new CheckTypesPass(),
        ];
    }

    public void Check(Program program)
    {
        foreach (AbstractPass pass in _passes)
        {
            program.Accept(pass);
        }
    }
}