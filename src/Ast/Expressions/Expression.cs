using PsTiger.Ast.Attributes;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Expressions;

/// <summary>
/// Абстрактный базовый класс для всех выражений.
/// </summary>
public abstract class Expression : AstNode
{
    private AstAttribute<ValueType> _resultType;

    /// <summary>
    /// Тип результата вычисления выражения.
    /// </summary>
    public ValueType ResultType
    {
        get => _resultType.Get();
        set => _resultType.Set(value);
    }
}