using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Semantics.Helpers;

/// <summary>
/// Вспомогательные методы для работы с типами данных языка.
/// </summary>
public static class ValueTypeUtil
{
    public static bool AreCompatibleTypes(ValueType a, ValueType b)
    {
        return a == b;
    }
}