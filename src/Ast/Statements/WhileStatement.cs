using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции while.
/// </summary>
public sealed class WhileStatement : Statement
{
    public WhileStatement(AstExpression condition, AbstractStatement body)
    {
        Condition = condition;
        Body = body;
    }

    public AstExpression Condition { get; }
    public AbstractStatement Body { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}