using PsTiger.Ast.Attributes;
using PsTiger.Ast.Statements;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Declarations;

public abstract class Declaration : Statement
{
    private readonly AstAttribute<ValueType> _resultType;
    private readonly string _name;

    protected Declaration(string name)
    {
        _name = name;
        _resultType = default;
    }

    public string Name => _name;

    public ValueType ResultType
    {
        get => _resultType.Get();
        set => _resultType.Set(value);
    }
}