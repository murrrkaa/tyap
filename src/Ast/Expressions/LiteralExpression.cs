using Mlt.Runtime;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Expressions;

public sealed class LiteralExpression : Expression
{
    public LiteralExpression(ValueType type, Value value)
    {
        Value = value;
        ResultType = type;
    }

    public Value Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}