using System.Linq.Expressions;

using Mlt.Ast;
using Mlt.Ast.Declarations;

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