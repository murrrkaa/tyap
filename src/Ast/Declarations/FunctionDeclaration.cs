using PsTiger.Ast.Attributes;
using PsTiger.Ast.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PsTiger.Ast.Declarations;

/// <summary>
/// Объявление пользовательской функции или процедуры.
/// </summary>
public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> _declaredType;

    public FunctionDeclaration(
        string name,
        IReadOnlyList<ParameterDeclaration> parameters,
        string? declaredTypeName,
        IReadOnlyList<AstNode> statements
    )
        : base(name, parameters)
    {
        DeclaredTypeName = declaredTypeName;
        Statements = statements;
    }

    public string? DeclaredTypeName { get; }

    public AbstractTypeDeclaration? DeclaredType
    {
        get => _declaredType.Get();
        set => _declaredType.Set(value);
    }

    public IReadOnlyList<AstNode> Statements { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}