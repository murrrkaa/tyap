namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции break.
/// </summary>
public sealed class BreakStatement : Statement
{
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}