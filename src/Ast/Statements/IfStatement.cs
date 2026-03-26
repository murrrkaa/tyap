using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции if-else.
/// </summary>
public sealed class IfStatement : Statement
{
    public IfStatement(AstExpression condition, AbstractStatement thenBranch, AbstractStatement? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public AstExpression Condition { get; }
    public AbstractStatement ThenBranch { get; }
    public AbstractStatement? ElseBranch { get; } // Может быть null

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}