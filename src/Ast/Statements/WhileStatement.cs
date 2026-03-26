using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции while.
/// </summary>
public sealed class WhileStatement : Statement
{
    public WhileStatement(Expression condition, Statement body)
    {
        Condition = condition;
        Body = body;
    }

    public Expression Condition { get; }
    public Statement Body { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}