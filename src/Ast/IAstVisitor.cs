using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;

namespace PsTiger.Ast;

public interface IAstVisitor
{
    void Visit(LiteralExpression e);

    void Visit(BinaryOperationExpression e);

    void Visit(SequenceExpression e);

    void Visit(UnaryMinusExpression e);

    void Visit(FunctionCallExpression e);

    void Visit(ScopeExpression e);

    void Visit(VariableAccessExpression e);

    void Visit(AssignmentExpression e);

    void Visit(IfElseStatement node);

    void Visit(VariableDeclaration d);

    void Visit(ConstantDeclaration d); // <- добавила

    void Visit(FunctionDeclaration d);

    void Visit(ParameterDeclaration d);

    void Visit(WhileStatement node);

    void Visit(ForStatement node);

    void Visit(ForIteratorDeclaration d);

    void Visit(BreakLoopExpression e);

    void Visit(ContinueStatement e); // <- добавила

    void Visit(TypeDeclaration d);

    void Visit(NamedTypeExpression e);

    void Visit(PrintStatement e); // <- добавила
}