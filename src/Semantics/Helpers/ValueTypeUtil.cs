using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Semantics.Helpers;

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