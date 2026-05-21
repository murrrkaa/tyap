using Mlt.Ast.Attributes;
using Mlt.Ast.Statements;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Declarations;

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