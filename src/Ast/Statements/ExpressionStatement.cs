using Mlt.Ast.Expressions;

namespace Mlt.Ast.Statements;

public class ExpressionStatement : Statement
{
	public ExpressionStatement(Expression expression)
	{
		Expression = expression;
	}

	public Expression Expression { get; }

	public override void Accept(IAstVisitor visitor)
	{
		visitor.Visit(this);
	}
}