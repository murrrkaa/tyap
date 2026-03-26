using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Объявление инструкции вывода.
/// </summary>
public sealed class PrintStatement : Statement
{
    public PrintStatement(AstExpression expression)
    {
        Expression = expression;
    }

    public AstExpression Expression { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}