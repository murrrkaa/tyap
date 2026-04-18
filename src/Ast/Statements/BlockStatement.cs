using System.Collections.Generic;

namespace Mlt.Ast.Statements;

/// <summary>
/// Объявление инструкции блока кода.
/// </summary>
public sealed class BlockStatement : Statement
{
    public BlockStatement(IReadOnlyList<AstNode> nodes)
    {
        Nodes = nodes;
    }

    public IReadOnlyList<AstNode> Nodes { get; }

    public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
}