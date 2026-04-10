using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции if-else.
/// </summary>
public sealed class IfStatement : Statement
{
    public IfStatement(Expression condition, Statement thenBranch, Statement? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expression Condition { get; }

    public Statement ThenBranch { get; }

    public Statement? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}