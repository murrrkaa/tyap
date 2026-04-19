using Mlt.Ast.Attributes;
using Mlt.Ast.Statements;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Declarations;

public abstract class Declaration : Statement
{
    private AstAttribute<ValueType> _resultType;
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