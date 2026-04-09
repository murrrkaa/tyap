using System;
using System.Globalization;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Runtime;

public class Value : IEquatable<Value>
{
    public static readonly Value Void = new(VoidValue.Value);

    private readonly object _value;

    #region Конструкторы

    public Value(string value)
    {
        _value = value;
    }

    public Value(int value)
    {
        _value = value;
    }

    public Value(double value)
    {
        _value = value;
    }

    public Value(bool value)
    {
        _value = value;
    }

    private Value(object value)
    {
        _value = value;
    }

    #endregion

    #region Проверка типов

    public bool IsVoid() => _value is VoidValue;

    public bool IsString() => _value is string;

    public bool IsInt() => _value is int;

    public bool IsFloat() => _value is double;

    public bool IsBool() => _value is bool;

    #endregion

    #region Получение значения

    public string AsString()
    {
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value is not a string: {_value}")
        };
    }

    public int AsInt()
    {
        return _value switch
        {
            int i => i,
            _ => throw new InvalidOperationException($"Value is not an integer: {_value}")
        };
    }

    public double AsFloat()
    {
        return _value switch
        {
            double f => f,
            _ => throw new InvalidOperationException($"Value is not a float: {_value}")
        };
    }

    public bool AsBool()
    {
        return _value switch
        {
            bool b => b,
            _ => throw new InvalidOperationException($"Value is not a boolean: {_value}")
        };
    }

    #endregion

    #region Сравнение

    public bool LessThan(Value other)
    {
        return _value switch
        {
            int i when other.IsInt() => i < other.AsInt(),
            double f when other.IsFloat() => f < other.AsFloat(),
            string s when other.IsString() => string.CompareOrdinal(s, other.AsString()) < 0,
            _ => throw new InvalidOperationException($"Cannot compare {_value} with {other._value}")
        };
    }

    public bool LessThanOrEqual(Value other)
    {
        return _value switch
        {
            int i when other.IsInt() => i <= other.AsInt(),
            double f when other.IsFloat() => f <= other.AsFloat(),
            string s when other.IsString() => string.CompareOrdinal(s, other.AsString()) <= 0,
            _ => throw new InvalidOperationException($"Cannot compare {_value} with {other._value}")
        };
    }

    public bool GreaterThan(Value other) => !LessThanOrEqual(other);

    public bool GreaterThanOrEqual(Value other) => !LessThan(other);

    #endregion

    #region Равенство

    public bool Equals(Value? other)
    {
        if (other is null) return false;
        if (IsVoid() && other.IsVoid()) return true;

        return _value switch
        {
            string s => other.IsString() && other.AsString() == s,
            int i => other.IsInt() && other.AsInt() == i,
            double f => other.IsFloat() && other.AsFloat() == f,
            bool b => other.IsBool() && other.AsBool() == b,
            VoidValue => other.IsVoid(),
            _ => false
        };
    }

    public override bool Equals(object? obj) => Equals(obj as Value);

    public override int GetHashCode() => _value?.GetHashCode() ?? 0;

    #endregion

    #region Вспомогательные

    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            int i => i.ToString(CultureInfo.InvariantCulture),
            double f => f.ToString(CultureInfo.InvariantCulture),
            bool b => b ? "true" : "false",
            VoidValue => "void",
            _ => _value?.ToString() ?? "null"
        };
    }

    #endregion
}