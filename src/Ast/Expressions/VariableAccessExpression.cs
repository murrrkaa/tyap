using System.Linq.Expressions;

using Mlt.Ast.Attributes;

using Mlt.Ast.Declarations;

namespace Mlt.Ast.Expressions;

/// <summary>
/// Выражение доступа к переменной по имени.
/// </summary>
public sealed class VariableAccessExpression : Expression
{
    private AstAttribute<AbstractVariableDeclaration> _variable;

    public VariableAccessExpression(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public AbstractVariableDeclaration Variable
    {
        get => _variable.Get();
        set => _variable.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}