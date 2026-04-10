using PsTiger.Ast.Statements;
using PsTiger.Runtime;
using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Declarations;

public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(
        string name,
        BlockStatement body
    )
        : base(name) 
    {
        if (name != "main")
        {
            throw new System.ArgumentException("For Epic 1, only 'main' function is supported.");
        }

        Body = body;

        ResultType = ValueType.Int;
    }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}