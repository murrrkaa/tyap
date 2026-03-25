using System.Linq.Expressions;

namespace PsTiger.Ast.Declarations;

/// <summary>
/// Узел дерева, представляющий объявление константы (const).
/// У константы указан неизменяемый тип.
/// </summary>
public sealed class ConstantDeclaration : AbstractVariableDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> _declaredType;

    public ConstantDeclaration(string name, string? declaredTypeName, Expression initialValue)
        : base(name)
    {
        DeclaredTypeName = declaredTypeName;
        InitialValue = initialValue;
    }

    public string? DeclaredTypeName { get; }

    public Expression InitialValue { get; }

    public AbstractTypeDeclaration? DeclaredType
    {
        get => _declaredType.Get();
        set => _declaredType.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        // Нужно будет добавить метод Visit(ConstantDeclaration node) в интерфейс IAstVisitor
        visitor.Visit(this);
    }
}