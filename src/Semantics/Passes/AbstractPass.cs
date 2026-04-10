using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;

namespace PsTiger.Semantics.Passes;

public abstract class AbstractPass : IAstVisitor
{
    public virtual void Visit(Program node)
    {
        foreach (Declaration declaration in node.TopLevelStatements)
        {
            declaration.Accept(this);
        }

        node.MainFunction.Accept(this);
    }

    public virtual void Visit(LiteralExpression e)
    {
    }

    public virtual void Visit(VariableAccessExpression e)
    {
    }

    public virtual void Visit(FunctionCallExpression e)
    {
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
    }

    public virtual void Visit(UnaryNotExpression e)
    {
        e.Operand.Accept(this);
    }

    public virtual void Visit(BlockStatement node)
    {
        foreach (AstNode astNode in node.Nodes)
        {
            if (astNode is Statement statement)
            {
                statement.Accept(this);
            }
            else if (astNode is Declaration declaration)
            {
                declaration.Accept(this);
            }
        }
    }

    public virtual void Visit(AssignmentStatement node)
    {
        node.Value.Accept(this);
    }

    public virtual void Visit(IfStatement node)
    {
        node.Condition.Accept(this);
        node.ThenBranch.Accept(this);
        node.ElseBranch?.Accept(this);
    }

    public virtual void Visit(WhileStatement node)
    {
        node.Condition.Accept(this);
        node.Body.Accept(this);
    }

    public virtual void Visit(ForStatement node)
    {
        node.Init.Accept(this);
        node.Condition.Accept(this);
        node.Step.Accept(this);
        node.Body.Accept(this);
    }

    public virtual void Visit(BreakStatement node)
    {
    }

    public virtual void Visit(ContinueStatement node)
    {
    }

    public virtual void Visit(ReturnStatement node)
    {
        node.Expression?.Accept(this);
    }

    public virtual void Visit(PrintStatement node)
    {
        foreach (Expression argument in node.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(FunctionCallStatement node)
    {
        node.Call.Accept(this);
    }

    public virtual void Visit(VariableDeclaration d)
    {
        d.InitialValue.Accept(this);
    }

    public virtual void Visit(ConstantDeclaration d)
    {
        d.InitialValue.Accept(this);
    }

    public virtual void Visit(ParameterDeclaration d)
    {
    }

    public virtual void Visit(FunctionDeclaration d)
    {
        foreach (AbstractParameterDeclaration parameter in d.Parameters)
        {
            parameter.Accept(this);
        }

        d.Body.Accept(this);
    }

    public virtual void Visit(BuiltinFunction d)
    {
    }

    public virtual void Visit(BuiltinFunctionParameter d)
    {
    }
}