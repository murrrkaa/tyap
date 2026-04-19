using Mlt.Ast.Statements;
using Mlt.Runtime;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Declarations;

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