using Mlt.Ast;
using Mlt.Semantics.Passes;

namespace Mlt.Semantics;

public class SemanticsChecker
{
    private readonly AbstractPass[] _passes;

    public SemanticsChecker()
    {
        _passes =
        [
            new ResolveNamesPass(), 
           
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