using PsTiger.Runtime;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Semantics.Helpers;

public static class ValueTypeUtil
{
    /// <summary>
    /// Проверяет, можно ли значение типа 'from' присвоить в переменную типа 'to'.
    /// </summary>
    public static bool AreCompatibleTypes(ValueType to, ValueType from)
    {
        if (to == from) return true;

        // Разрешаем неявное приведение int к float (например: float f = 5)
        if (to == ValueType.Float && from == ValueType.Int) return true;

        return false;
    }

    /// <summary>
    /// Определяет общий тип для двух выражений (используется в if-else и арифметике).
    /// </summary>
    public static ValueType GetCommonType(ValueType a, ValueType b)
    {
        if (a == b) return a;

        // Если смешиваем int и float, результат всегда float
        if ((a == ValueType.Int || a == ValueType.Float) &&
            (b == ValueType.Int || b == ValueType.Float))
        {
            return ValueType.Float;
        }

        // Если типы совсем разные (например, string и int)
        return ValueType.Void;
    }

    /// <summary>
    /// Преобразует строковое представление типа из исходного кода в ValueType.
    /// </summary>
    public static ValueType Parse(string typeName)
    {
        return typeName.ToLower() switch
        {
            "int" => ValueType.Int,
            "float" => ValueType.Float,
            "string" => ValueType.String,
            "bool" => ValueType.Bool,
            "void" => ValueType.Void,
            _ => throw new Exception($"Unknown type name: {typeName}")
        };
    }
}