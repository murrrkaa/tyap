using PsTiger.Ast.Statements;
using PsTiger.Runtime;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast.Declarations;

public sealed class MainFunctionDeclaration : Declaration
{
    public MainFunctionDeclaration(BlockStatement body)
        : base("main")
    {
        Body = body;
        ResultType = ValueType.Int;
    }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}