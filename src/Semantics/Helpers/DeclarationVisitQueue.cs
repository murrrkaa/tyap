using Mlt.Ast;
using Mlt.Ast.Declarations;

namespace Mlt.Semantics.Helpers;

public class DeclarationVisitQueue
{
    private readonly IAstVisitor _visitor;
    private readonly Queue<MainFunctionDeclaration> _visitQueue;
    private bool _isQueueing;

    public DeclarationVisitQueue(IAstVisitor visitor)
    {
        _visitor = visitor;
        _visitQueue = [];
        _isQueueing = false;
    }

    public void BeginFunctionGroup()
    {
        if (!_isQueueing)
        {
            Flush();
            _isQueueing = true;
        }
    }

    public void EndFunctionGroup()
    {
        if (_isQueueing)
        {
            Flush();
            _isQueueing = false;
        }
    }

    public void Enqueue(MainFunctionDeclaration declaration)
    {
        if (_isQueueing)
        {
            _visitQueue.Enqueue(declaration);
        }
        else
        {
            declaration.Accept(_visitor);
        }
    }

    private void Flush()
    {
        while (_visitQueue.TryDequeue(out MainFunctionDeclaration? declaration))
        {
            declaration.Accept(_visitor);
        }
    }
}