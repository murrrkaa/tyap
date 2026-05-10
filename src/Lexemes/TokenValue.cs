using System;
using System.Globalization;

namespace Mlt.Lexemes;

public sealed class TokenValue
{
    private readonly object _value;

    public TokenValue(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public TokenValue(int value)
    {
        _value = (decimal)value;
    }

    public TokenValue(double value)
    {
        _value = (decimal)value;
    }

    public TokenValue(decimal value)
    {
        _value = value;
    }

    public bool IsString() => _value is string;
    public bool IsInt() => _value is decimal;
    public bool IsDecimal() => _value is decimal;

    public string AsString()
    {
        return _value as string ?? throw new InvalidCastException("TokenValue is not a string");
    }

    public int AsInt()
    {
        if (_value is decimal d) return (int)d;
        throw new InvalidCastException("TokenValue is not a number");
    }

    public decimal AsDecimal()
    {
        if (_value is decimal d) return d;
        throw new InvalidCastException("TokenValue is not a decimal");
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            decimal dec => dec.ToString(CultureInfo.InvariantCulture),
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