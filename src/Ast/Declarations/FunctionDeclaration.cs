using Mlt.Ast.Statements;

using Mlt.Runtime;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Declarations;

public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
    public FunctionDeclaration(
        string name,
        IReadOnlyList<ParameterDeclaration> parameters,
        string? declaredReturnType,
        ValueType resolvedType,
        BlockStatement body
    )
        : base(name, parameters.Cast<AbstractParameterDeclaration>().ToList())
    {
        DeclaredReturnTypeName = declaredReturnType;
        ResolvedReturnType = resolvedType;
        Body = body;
    }

    public string? DeclaredReturnTypeName { get; }

    public BlockStatement Body { get; }

    public ValueType ResolvedReturnType { get; set; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}