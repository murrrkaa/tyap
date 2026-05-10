using Mlt.Ast.Expressions;

namespace Mlt.Ast.Statements;

/// <summary>
/// Литеральное значение (число, строка или nil).
/// </summary>
public class ExpressionStatement : Statement
{
    public ExpressionStatement(Expression expression)
    {
        Expression = expression;
    }

    public Expression Expression { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}