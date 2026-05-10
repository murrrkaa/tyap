using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;

namespace Mlt.Ast;

public interface IAstVisitor
{
    void Visit(LiteralExpression e);

    void Visit(BinaryOperationExpression e);

    void Visit(VariableAccessExpression e);

    void Visit(AssignmentExpression e);

    void Visit(ExpressionStatement node);

    void Visit(BlockStatement node);

    void Visit(PrintStatement node);

    void Visit(ReturnStatement node);

    void Visit(MainFunctionDeclaration d);

    void Visit(VariableDeclaration d);

    void Visit(Program node);

}