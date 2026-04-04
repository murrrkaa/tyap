namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции continue.
/// </summary>
public sealed class ContinueStatement : Statement
{
    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}