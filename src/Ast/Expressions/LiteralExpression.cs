using PsTiger.Runtime;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Expressions;

/// <summary>
/// Литеральное значение: число, строка или булево.
/// </summary>
public sealed class LiteralExpression : Expression
{
    public LiteralExpression(ValueType type, Value value)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Тип литерала (int, float, string, bool).
    /// </summary>
    public ValueType Type { get; }

    /// <summary>
    /// Значение литерала.
    /// </summary>
    public Value Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}