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

    public virtual void Visit(MainFunctionDeclaration node)
    {
        node.Body.Accept(this);
    }

    public virtual void Visit(FunctionDeclaration node)
    {
        foreach (ParameterDeclaration param in node.Parameters.OfType<ParameterDeclaration>())
        {
            param.Accept(this);
        }

        node.Body.Accept(this);
    }

    public virtual void Visit(BlockStatement node)
    {
        foreach (AstNode nodeItem in node.Nodes)
        {
            nodeItem.Accept(this);
        }
    }

    public virtual void Visit(PrintStatement node)
    {
        foreach (Expression argument in node.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(VariableDeclaration node)
    {
        node.InitialValue?.Accept(this);
    }

    public virtual void Visit(ConstantDeclaration node)
    {
        node.InitialValue?.Accept(this);
    }

    public virtual void Visit(AssignmentStatement node)
    {
        node.Value.Accept(this);
    }

    public virtual void Visit(BinaryOperationExpression node)
    {
        node.Left.Accept(this);
        node.Right.Accept(this);
    }

    public virtual void Visit(UnaryNotExpression node)
    {
        node.Operand.Accept(this);
    }

    public virtual void Visit(FunctionCallExpression node)
    {
        foreach (Expression arg in node.Arguments)
        {
            arg.Accept(this);
        }
    }

    public virtual void Visit(FunctionCallStatement node)
    {
        node.Call.Accept(this);
    }

    public virtual void Visit(ExpressionStatement node)
    {
        node.Expression.Accept(this);
    }

    public virtual void Visit(ReturnStatement node)
    {
        node.Expression?.Accept(this);
    }

    public virtual void Visit(ParameterDeclaration node)
    {
    }

    public virtual void Visit(BuiltinFunction node)
    {
    }

    public virtual void Visit(BuiltinFunctionParameter node)
    {
    }

    public virtual void Visit(VariableAccessExpression node)
    {
    }

    public virtual void Visit(LiteralExpression node)
    {
    }
}