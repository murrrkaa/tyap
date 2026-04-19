using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;

namespace Mlt.Semantics.Passes;

public abstract class AbstractPass : IAstVisitor
{
    public virtual void Visit(Program node)
    {
        node.MainFunction.Accept(this);
    }

    public virtual void Visit(MainFunctionDeclaration d)
    {
        d.Body.Accept(this);
    }

    public virtual void Visit(BlockStatement node)
    {
        foreach (Statement statement in node.Nodes)
        {
            statement.Accept(this);
        }
    }

    public virtual void Visit(PrintStatement node)
    {
        foreach (Expression argument in node.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(ReturnStatement node)
    {
        node.Expression?.Accept(this);
    }

    public virtual void Visit(LiteralExpression e)
    {
    }
}