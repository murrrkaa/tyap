using System.Globalization;

namespace Mlt.Runtime;

public class Value : IEquatable<Value>
{
    private readonly object _value;

    public Value(string value)
    {
        _value = value;
    }

    public Value(decimal value)
    {
        _value = value;
    }

    public Value(long value)
    {
        _value = value;
    }

    public bool IsString() => _value is string;

    public bool IsInt() => _value is long;

    public bool IsFloat() => _value is decimal;

    public string AsString()
    {
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {_value} is not a string"),
        };
    }

    public decimal AsDecimal()
    {
        return _value switch
        {
            decimal d => d,
            long l => (decimal)l,
            _ => throw new InvalidOperationException($"Value {_value} is not a number"),
        };
    }

    public long AsLong()
    {
        return _value switch
        {
            long l => l,
            _ => throw new InvalidOperationException($"Value {_value} is not an int"),
        };
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            long l => l.ToString(CultureInfo.InvariantCulture),
            decimal d => d.ToString(CultureInfo.InvariantCulture),
            _ => _value?.ToString() ?? "null",
        };
    }

    public bool Equals(Value? other)
    {
        if (other is null) return false;

        return _value switch
        {
            string s => other.IsString() && other.AsString() == s,
            long l => other.IsInt() && other.AsLong() == l,
            decimal d => other.IsFloat() && other.AsDecimal() == d,
            _ => Equals(_value, other._value),
        };
    }

    public override bool Equals(object? obj) => Equals(obj as Value);

    public override int GetHashCode() => _value.GetHashCode();
}