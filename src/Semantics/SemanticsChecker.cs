using PsTiger.Ast;
using PsTiger.Semantics.Passes;

namespace PsTiger.Semantics;

public class SemanticsChecker
{
    private readonly AbstractPass _pass;

    public SemanticsChecker()
    {
        _pass = new CheckTypesPass();
    }

    public void Check(Program program)
    {
        program.Accept(_pass);
    }
}