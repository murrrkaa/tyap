using PsTiger.Ast;
using PsTiger.Semantics.Passes;

namespace PsTiger.Semantics;

public class SemanticsChecker
{
    private readonly AbstractPass[] _passes;

    public SemanticsChecker()
    {
        _passes = new AbstractPass[]
        {
            new CheckTypesPass(),
        };
    }

    public void Check(Program program)
    {
        foreach (var pass in _passes)
        {
            program.Accept(pass);
        }
    }
}
