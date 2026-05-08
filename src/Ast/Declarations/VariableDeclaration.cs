using Mlt.Ast.Expressions;
using Mlt.Runtime;

using ValueType = Mlt.Runtime.ValueType;

namespace Mlt.Ast.Declarations;

public sealed class VariableDeclaration : Declaration
{
	public VariableDeclaration(string name, ValueType type, Expression initializer, bool isMutable)
		: base(name)
	{
		Type = type;
		Initializer = initializer;
		IsMutable = isMutable;
	}

	public ValueType Type { get; }

	public Expression Initializer { get; }

	public bool IsMutable { get; }

	public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}