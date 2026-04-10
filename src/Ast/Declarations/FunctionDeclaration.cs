using PsTiger.Ast.Statements;

using PsTiger.Runtime;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Declarations;

public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(
        string name,
        BlockStatement body,
        ValueType resolvedType = ValueType.Void
    )
        : base(name)
    {
        ResolvedReturnType = resolvedType;
        Body = body;
    }

    public string? DeclaredReturnTypeName { get; }

    public BlockStatement Body { get; }

    public ValueType ResolvedReturnType { get; set; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}