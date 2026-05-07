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

    public Value(int value)
    {
        _value = (decimal)value;
    }

    public bool IsString()
    {
        return _value is string;
    }

    public bool IsInt()
    {
        return _value is decimal;
    }

    public bool IsFloat()
    {
        return _value is decimal;
    }

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
            _ => throw new InvalidOperationException($"Value {_value} is not a number"),
        };
    }

    public int AsInt()
    {
        return (int)AsDecimal();
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
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
            decimal d => (other.IsInt() || other.IsFloat()) && other.AsDecimal() == d,
            _ => Equals(_value, other._value),
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}