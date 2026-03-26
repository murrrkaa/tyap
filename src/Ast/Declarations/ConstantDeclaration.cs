using PsTiger.Ast.Expressions;

namespace PsTiger.Ast.Declarations;

/// <summary>
/// Объявление константы: const name: type = value.
/// </summary>
public sealed class ConstantDeclaration : AbstractVariableDeclaration
{
    public ConstantDeclaration(string name, string type, Expression initialValue)
        : base(name)
    {
        Name = name;
        Type = type;
        InitialValue = initialValue;
    }

    public new string Name { get; }
    public string Type { get; }  // ✅ Просто строка: "int", "float", etc.
    public Expression InitialValue { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}