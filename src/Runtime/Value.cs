using System;
using System.Globalization;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Runtime;

/// <summary>
/// Представляет значение времени выполнения языка.
/// Поддерживает только базовые типы: int, float, string, bool, void.
/// </summary>
public class Value : IEquatable<Value>
{
    /// <summary>
    /// Специальное значение для функций, не возвращающих результат.
    /// </summary>
    public static readonly Value Void = new(VoidValue.Value);

    private readonly object _value;

    #region Конструкторы

    /// <summary>
    /// Создаёт строковое значение.
    /// </summary>
    public Value(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Создаёт целочисленное значение.
    /// </summary>
    public Value(int value)
    {
        _value = value;
    }

    /// <summary>
    /// Создаёт значение с плавающей точкой.
    /// </summary>
    public Value(double value)
    {
        _value = value;
    }

    /// <summary>
    /// Создаёт булево значение.
    /// </summary>
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

    /// <summary>
    /// Определяет, является ли значение типа void.
    /// </summary>
    public bool IsVoid() => _value is VoidValue;

    /// <summary>
    /// Определяет, является ли значение строкой.
    /// </summary>
    public bool IsString() => _value is string;

    /// <summary>
    /// Определяет, является ли значение целым числом.
    /// </summary>
    public bool IsInt() => _value is int;

    /// <summary>
    /// Определяет, является ли значение числом с плавающей точкой.
    /// </summary>
    public bool IsFloat() => _value is double;

    /// <summary>
    /// Определяет, является ли значение булевым.
    /// </summary>
    public bool IsBool() => _value is bool;

    #endregion

    #region Получение значения

    /// <summary>
    /// Возвращает значение как строку либо бросает исключение.
    /// </summary>
    public string AsString()
    {
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value is not a string: {_value}")
        };
    }

    /// <summary>
    /// Возвращает значение как целое число либо бросает исключение.
    /// </summary>
    public int AsInt()
    {
        return _value switch
        {
            int i => i,
            _ => throw new InvalidOperationException($"Value is not an integer: {_value}")
        };
    }

    /// <summary>
    /// Возвращает значение как число с плавающей точкой либо бросает исключение.
    /// </summary>
    public double AsFloat()
    {
        return _value switch
        {
            double f => f,
            _ => throw new InvalidOperationException($"Value is not a float: {_value}")
        };
    }

    /// <summary>
    /// Возвращает значение как булево либо бросает исключение.
    /// </summary>
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

    /// <summary>
    /// Сравнивает два значения: текущее &lt; other.
    /// Поддерживает int, float, string.
    /// </summary>
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

    /// <summary>
    /// Сравнивает два значения: текущее &lt;= other.
    /// </summary>
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

    /// <summary>
    /// Сравнивает два значения: текущее &gt; other.
    /// </summary>
    public bool GreaterThan(Value other) => !LessThanOrEqual(other);

    /// <summary>
    /// Сравнивает два значения: текущее &gt;= other.
    /// </summary>
    public bool GreaterThanOrEqual(Value other) => !LessThan(other);

    #endregion

    #region Равенство

    /// <summary>
    /// Сравнивает значения на равенство.
    /// </summary>
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

    /// <summary>
    /// Возвращает строковое представление значения.
    /// </summary>
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