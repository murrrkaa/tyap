using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;

namespace PsTiger.Ast;

/// <summary>
/// Интерфейс для обхода дерева синтаксического анализа (паттерн Visitor).
/// </summary>
public interface IAstVisitor
{
    // === Expressions ===

    void Visit(LiteralExpression e);
    void Visit(VariableAccessExpression e);
    void Visit(FunctionCallExpression e);
    void Visit(BinaryOperationExpression e);
    void Visit(UnaryNotExpression e);

    // === Statements ===

    void Visit(BlockStatement node);
    void Visit(AssignmentStatement node);
    void Visit(IfStatement node);
    void Visit(WhileStatement node);
    void Visit(ForStatement node);
    void Visit(BreakStatement node);
    void Visit(ContinueStatement node);
    void Visit(ReturnStatement node);
    void Visit(PrintStatement node);
    void Visit(FunctionCallStatement node);

    // === Declarations ===

    void Visit(VariableDeclaration d);
    void Visit(ConstantDeclaration d);
    void Visit(ParameterDeclaration d);
    void Visit(FunctionDeclaration d);
    void Visit(BuiltinFunction d);
    void Visit(BuiltinFunctionParameter d);

    // === Program ===

    void Visit(Program node);
}