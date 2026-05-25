using Mlt.Ast.Expressions;

namespace Mlt.Ast.Statements;

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

    public FunctionCallExpression Call { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}