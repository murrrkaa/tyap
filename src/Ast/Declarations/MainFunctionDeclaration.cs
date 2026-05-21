using Mlt.Ast.Statements;
using Mlt.Runtime;

using Mlt.Ast; // Подключаем, чтобы визитор был из правильного namespace

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Declarations;

public sealed class MainFunctionDeclaration : Declaration
{
    public MainFunctionDeclaration(BlockStatement body) : base()
    {
        Body = body;
        ReturnType = ValueType.Int;
    }

    public BlockStatement Body { get; }
    public ValueType ReturnType { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}