using PsTiger.Ast.Expressions;
using System.Collections.Generic;

namespace PsTiger.Ast.Statements;

/// <summary>
/// Инструкция вывода: print(expr1, expr2, ...);
/// </summary>
public sealed class PrintStatement : Statement
{
    public PrintStatement(IReadOnlyList<Expression> arguments)
    {
        Arguments = arguments;
    }

    /// <summary>
    /// Список аргументов для вывода.
    /// </summary>
    public IReadOnlyList<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}