using Mlt.Ast.Declarations;

namespace Mlt.Ast;

public class Program : AstNode
{
    public Program(MainFunctionDeclaration mainFunction)
    {
        MainFunction = mainFunction;
    }

    public MainFunctionDeclaration MainFunction { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}