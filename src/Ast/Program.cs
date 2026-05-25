using System.Collections.Generic;

using Mlt.Ast.Declarations;

namespace Mlt.Ast;

/// <summary>
/// Корневой узел программы.
/// Содержит список верхнеуровневых объявлений и обязательную функцию main.
/// </summary>
public sealed class Program : AstNode
{
    public Program(
        IReadOnlyList<Declaration> topLevelStatements,
        MainFunctionDeclaration mainFunction
    )
    {
        TopLevelStatements = topLevelStatements;
        MainFunction = mainFunction;
    }

    public IReadOnlyList<Declaration> TopLevelStatements { get; }

    public MainFunctionDeclaration MainFunction { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}