using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Declarations;

/// <summary>
/// Объявление параметра функции.
/// </summary>
public class ParameterDeclaration : AbstractParameterDeclaration
{
    public ParameterDeclaration(string name, string typeName, ValueType resolvedType)
        : base(name)
    {
        TypeName = typeName;
        ResolvedType = resolvedType;
    }

    public string TypeName { get; }

    public ValueType ResolvedType { get; set; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}