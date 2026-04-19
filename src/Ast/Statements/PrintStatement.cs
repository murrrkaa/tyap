using System.Collections.Generic;

using Mlt.Ast.Expressions;

namespace Mlt.Ast.Statements;

/// <summary>
/// Инструкция вывода
/// </summary>
public sealed class PrintStatement : Statement
{
    public PrintStatement(IReadOnlyList<Expression> arguments)
    {
        Arguments = arguments;
    }

    public IReadOnlyList<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}