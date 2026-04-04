using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции for.
/// </summary>
public sealed class ForStatement : Statement
{
    public ForStatement(
        AssignmentStatement init,
        Expression condition,
        AssignmentStatement step,
        Statement body)
    {
        Init = init;
        Condition = condition;
        Step = step;
        Body = body;
    }

    public AssignmentStatement Init { get; }
    public Expression Condition { get; }
    public AssignmentStatement Step { get; }
    public Statement Body { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}