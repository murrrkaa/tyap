using PsTiger.Ast.Declarations;

namespace PsTiger.Ast;

public class Program : AstNode
{
    public Program(FunctionDeclaration mainFunction)
    {
        MainFunction = mainFunction;
    }

    public FunctionDeclaration MainFunction { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}