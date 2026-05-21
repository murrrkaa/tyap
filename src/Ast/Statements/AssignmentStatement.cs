using Mlt.Ast.Expressions;

namespace Mlt.Ast.Statements;

/// <summary>
/// Объявление инструкции присваивания.
/// </summary>
public sealed class AssignmentStatement : Statement
{
	public AssignmentStatement(string variableName, Expression value)
	{
		VariableName = variableName;
		Value = value;
	}

	public string VariableName { get; }

	public Expression Value { get; }

	public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}