using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции return.
/// </summary>
public sealed class ReturnStatement : Statement
{
    public ReturnStatement(AstExpression? expression)
    {
        Expression = expression; // Может быть null, если просто "return;"
    }

    public AstExpression? Expression { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}