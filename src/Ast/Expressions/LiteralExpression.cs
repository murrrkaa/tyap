using Mlt.Runtime;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Expressions;

/// <summary>
/// Литеральное значение (число, строка или nil).
/// </summary>
public sealed class LiteralExpression : Expression
{
    public LiteralExpression(ValueType type, Value value)
    {
        Type = type;
        Value = value;
    }

    public ValueType Type { get; }

    public Value Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}