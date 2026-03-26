using System.Runtime.CompilerServices;

namespace PsTiger.Runtime;

/// <summary>
/// Тип значения времени выполнения.
/// </summary>
public class ValueType
{
    /// <summary>
    /// Тип для функций, не возвращающих значение.
    /// </summary>
    public static readonly ValueType Void = new("void");

    /// <summary>
    /// 64-битное целое число со знаком.
    /// </summary>
    public static readonly ValueType Int = new("int");

    /// <summary>
    /// Число с плавающей точкой (десятичное).
    /// </summary>
    public static readonly ValueType Float = new("float");

    /// <summary>
    /// Строка (ASCII).
    /// </summary>
    public static readonly ValueType String = new("string");

    /// <summary>
    /// Булево значение (true / false).
    /// </summary>
    public static readonly ValueType Bool = new("bool");

    private readonly string _name;

    protected ValueType(string name)
    {
        _name = name;
    }

    public static bool operator ==(ValueType? a, ValueType? b) => a?.Equals(b) ?? b is null;

    public static bool operator !=(ValueType? a, ValueType? b) => !(a == b);

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    public override string ToString()
    {
        return _name;
    }
}