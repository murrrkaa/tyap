using PsTiger.Ast.Attributes;
using PsTiger.Ast.Expressions;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Declarations;

/// <summary>
///  Узел дерева, представляющий объявление переменной.
///  У переменной может быть указан тип и всегда указано начальное значение.
/// </summary>
public sealed class VariableDeclaration : AbstractVariableDeclaration
{
    public VariableDeclaration(string name, string declaredTypeName, ValueType resolvedType, Expression initialValue)
        : base(name)
    {
        DeclaredTypeName = declaredTypeName;
        ResolvedType = resolvedType;
        InitialValue = initialValue;
    }

    public string DeclaredTypeName { get; }

    public ValueType ResolvedType { get; set; }

    public Expression InitialValue { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}