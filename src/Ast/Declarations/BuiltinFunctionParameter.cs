using Mlt.Runtime;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Declarations;

public sealed class BuiltinFunctionParameter : AbstractParameterDeclaration
{
    public BuiltinFunctionParameter(string name, ValueType type)
        : base(name)
    {
        Type = type;
    }

    public ValueType Type { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}