using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Инструкция вызова функции как отдельное утверждение.
/// Например: print(x); или readInt();
/// </summary>
public sealed class FunctionCallStatement : Statement
{
    public FunctionCallStatement(FunctionCallExpression call)
    {
        Call = call;
    }

    /// <summary>
    /// Выражение вызова функции.
    /// </summary>
    public FunctionCallExpression Call { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}