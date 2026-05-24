using System;
using System.Globalization;

namespace Mlt.Runtime;

public class Value : IEquatable<Value>
{
    private readonly object _value;

    public Value(string value)
    {
        _value = value;
    }

    public Value(double value)
    {
        _value = value;
    }

    public Value(long value)
    {
        _value = value;
    }

    public Value(bool value)
    {
        _value = value;
    }

    public bool IsString() => _value is string;

    public bool IsInt() => _value is long;

    public bool IsFloat() => _value is double;

    public bool IsBool() => _value is bool;

    public string AsString()
    {
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Значение {_value} не является строкой."),
        };
    }

    public double AsDouble()
    {
        return _value switch
        {
            double d => d,
            long l => (double)l,
            _ => throw new InvalidOperationException($"\r\nЗначение {{_value}} не является числом с плавающей запятой."),
        };
    }

    public long AsLong()
    {
        return _value switch
        {
            long l => l,
            _ => throw new InvalidOperationException($"\r\nЗначение {{_value}} не является целым числом."),
        };
    }

    public bool AsBool()
    {
        return _value switch
        {
            bool b => b,
            _ => throw new InvalidOperationException($"Значение {_value} не является логическим значением."),
        };
    }

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            long l => l.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            bool b => b ? "true" : "false",
            _ => _value?.ToString() ?? "null",
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
            long l => other.IsInt() && other.AsLong() == l,
            double d => other.IsFloat() && other.AsDouble() == d,
            bool b => other.IsBool() && other.AsBool() == b,
            _ => Equals(_value, other._value),
        };
    }

    public override bool Equals(object? obj) => Equals(obj as Value);

    public override int GetHashCode() => _value.GetHashCode();
}