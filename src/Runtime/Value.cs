using System;
using System.Globalization;

namespace PsTiger.Runtime;

public class Value : IEquatable<Value>
{
    private readonly object _value;

    public Value(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Value(int value)
    {
        _value = value;
    }

    public Value(decimal value)
    {
        _value = value;
    }

    public bool IsString() => _value is string;

    public bool IsInt() => _value is int;

    public bool IsFloat() => _value is decimal;

    public string AsString()
    {
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value is not a string: {_value}"),
        };
    }

    public int AsInt()
    {
        return _value switch
        {
            int i => i,
            _ => throw new InvalidOperationException($"Value is not an integer: {_value}"),
        };
    }

    public decimal AsFloat()
    {
        return _value switch
        {
            decimal f => f,
            _ => throw new InvalidOperationException($"Value is not a float: {_value}"),
        };
    }

    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        return _value switch
        {
            string s => other.IsString() && other.AsString() == s,
            int i => other.IsInt() && other.AsInt() == i,
            decimal f => other.IsFloat() && other.AsFloat() == f,
            _ => false,
        };
    }

    public override bool Equals(object? obj) => Equals(obj as Value);

    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            int i => i.ToString(CultureInfo.InvariantCulture),
            decimal f => f.ToString(CultureInfo.InvariantCulture),
            _ => _value?.ToString() ?? "null",
        };
    }
}