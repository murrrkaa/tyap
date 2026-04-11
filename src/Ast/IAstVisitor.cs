using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;

namespace PsTiger.Ast;

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