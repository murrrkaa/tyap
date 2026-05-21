using System.Runtime.CompilerServices;

namespace Mlt.Runtime;

/// <summary>
/// Тип значения времени выполнения.
/// </summary>
public class ValueType
{
    public static readonly ValueType Int = new("int");
    public static readonly ValueType Float = new("float");
    public static readonly ValueType String = new("string");
    public static readonly ValueType Bool = new("bool");
    public static readonly ValueType Void = new("void");

    /// <summary>
    /// Специальный тип для параметров встроенных функций, принимающих любой тип.
    /// </summary>
    public static readonly ValueType Any = new("any");

    private readonly string _name;

    protected ValueType(string name) { _name = name; }

    public static bool operator ==(ValueType? a, ValueType? b) => a?.Equals(b) ?? b is null;
    public static bool operator !=(ValueType? a, ValueType? b) => !(a == b);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj);
    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    public override string ToString() => _name;
}