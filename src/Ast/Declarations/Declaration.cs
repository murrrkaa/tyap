using PsTiger.Ast.Attributes;
using PsTiger.Ast.Statements;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Declarations;

public abstract class Declaration : Statement
{
    private AstAttribute<ValueType> _resultType;

    /// <summary>
    /// Тип результата объявления.
    /// </summary>
    public ValueType ResultType
    {
        get => _resultType.Get();

        set => _resultType.Set(value);
    }
}