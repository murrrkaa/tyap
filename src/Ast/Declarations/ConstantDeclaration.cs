using PsTiger.Ast.Expressions;

using PsTiger.Runtime;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Declarations;

public sealed class ConstantDeclaration : AbstractVariableDeclaration
{
    public ConstantDeclaration(string name, string declaredTypeName, ValueType resolvedType, Expression initialValue)
        : base(name)
    {
        Name = name;
        DeclaredTypeName = declaredTypeName;
        ResolvedType = resolvedType;
        InitialValue = initialValue;
    }

    public new string Name { get; }

    public string DeclaredTypeName { get; }

    public ValueType ResolvedType { get; set; }

    public Expression InitialValue { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}