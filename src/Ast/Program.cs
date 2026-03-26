using PsTiger.Ast.Declarations;
using System.Collections.Generic;

namespace PsTiger.Ast;

/// <summary>
/// Корневой узел программы.
/// Содержит список верхнеуровневых объявлений и обязательную функцию main.
/// </summary>
public sealed class Program : AstNode
{
    public Program(
        IReadOnlyList<Declaration> topLevelStatements,
        FunctionDeclaration mainFunction
    )
    {
        TopLevelStatements = topLevelStatements;
        MainFunction = mainFunction;
    }

    public IReadOnlyList<Declaration> TopLevelStatements { get; }

    public FunctionDeclaration MainFunction { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}