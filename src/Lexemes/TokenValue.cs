using System;
using System.Globalization;

namespace PsTiger.Lexemes;

public sealed class TokenValue
{
    private readonly object _value;

    public TokenValue(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public TokenValue(int value)
    {
        _value = value;
    }

    public TokenValue(double value)
    {
        _value = value;
    }

    public bool IsString() => _value is string;

    public bool IsInt() => _value is int;

    public bool IsDouble() => _value is double;

    public string AsString()
    {
        return _value as string ?? throw new InvalidCastException("TokenValue is not a string");
    }

    public int AsInt()
    {
        return _value is int i ? i : throw new InvalidCastException("TokenValue is not an integer");
    }

    public double AsDouble()
    {
        return _value switch
        {
            double d => d,
            int i => i,
            _ => throw new InvalidCastException("TokenValue is not a number"),
        };
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            int i => i.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            _ => _value?.ToString() ?? "null",
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is TokenValue other)
        {
            return Equals(_value, other._value);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _value?.GetHashCode() ?? 0;
    }
}