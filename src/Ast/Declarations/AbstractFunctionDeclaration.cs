using System.Collections.Generic;

namespace Mlt.Ast.Declarations;

public abstract class AbstractFunctionDeclaration : Declaration
{
    protected AbstractFunctionDeclaration(
        string name,
        IReadOnlyList<AbstractParameterDeclaration> parameters
    )

    {
        Name = name;
        Parameters = parameters;
    }

    public string Name { get; }

    public IReadOnlyList<AbstractParameterDeclaration> Parameters { get; }
}