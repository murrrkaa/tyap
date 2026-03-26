using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции for.
/// </summary>
public sealed class ForStatement : Statement
{
    public ForStatement(
        AssignmentStatement init,
        AstExpression condition,
        AssignmentStatement step,
        AbstractStatement body)
    {
        Init = init;
        Condition = condition;
        Step = step;
        Body = body;
    }

    public AssignmentStatement Init { get; }
    public AstExpression Condition { get; }
    public AssignmentStatement Step { get; }
    public AbstractStatement Body { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}