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

    void Visit(VariableAccessExpression e);

    void Visit(FunctionCallExpression e);

    void Visit(BinaryOperationExpression e);

    void Visit(UnaryNotExpression e);

    void Visit(BlockStatement node);

    void Visit(AssignmentStatement node);

    void Visit(ExpressionStatement node);

    void Visit(ReturnStatement node);

    void Visit(PrintStatement node);

    void Visit(FunctionCallStatement node);

    void Visit(MainFunctionDeclaration d);

    void Visit(VariableDeclaration d);

    void Visit(ConstantDeclaration d);

    void Visit(ParameterDeclaration d);

    void Visit(FunctionDeclaration d);

    void Visit(BuiltinFunction d);

    void Visit(BuiltinFunctionParameter d);

    void Visit(Program node);
}