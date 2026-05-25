using Mlt.Ast;

namespace Mlt.Ast.Expressions;

public sealed class UnaryNotExpression : Expression
{
    public UnaryNotExpression(Expression operand)
    {
        Operand = operand;
    }

    public Expression Operand { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}