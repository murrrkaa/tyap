using Mlt.Ast;
using Mlt.Ast.Declarations;
using System.Linq.Expressions;

namespace Mlt.Ast.Expressions;

public sealed class VariableAccessExpression : Expression
{
    public VariableAccessExpression(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}