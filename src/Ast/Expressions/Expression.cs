using Mlt.Ast.Attributes;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Expressions;

/// <summary>
/// Абстрактный базовый класс для всех выражений.
/// </summary>
public abstract class Expression : AstNode
{
    private AstAttribute<ValueType> _resultType;

    public ValueType ResultType
    {
        get => _resultType.Get();
        set => _resultType.Set(value);
    }
}