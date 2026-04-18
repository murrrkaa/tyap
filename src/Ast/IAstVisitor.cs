using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;

namespace Mlt.Ast;

/// <summary>
/// Интерфейс для обхода дерева синтаксического анализа (паттерн Visitor).
/// </summary>
public interface IAstVisitor
{
    void Visit(LiteralExpression e);

    void Visit(BlockStatement node);

    void Visit(ReturnStatement node);

    void Visit(PrintStatement node);

    void Visit(MainFunctionDeclaration d);

    void Visit(Program node);
}