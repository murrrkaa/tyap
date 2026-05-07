using Mlt.Ast;
using Mlt.Ast.Declarations;

namespace Mlt.Semantics.Helpers;

/// <summary>
/// Обеспечивает обход объявлений. В Эпике №1 выполняет немедленный визит.
/// </summary>
public class DeclarationVisitQueue
{
    private readonly IAstVisitor _visitor;

    public DeclarationVisitQueue(IAstVisitor visitor)
    {
        _visitor = visitor;
    }

    public void Enqueue(Declaration declaration)
    {
        declaration.Accept(_visitor);
    }
}