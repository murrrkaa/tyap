using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции return.
/// </summary>
public sealed class ReturnStatement : Statement
{
    public ReturnStatement(Expression? expression)
    {
        Expression = expression;
    }

    public Expression? Expression { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}