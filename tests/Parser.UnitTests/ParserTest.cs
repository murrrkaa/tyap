using System.Linq;
using Mlt.Ast;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using Mlt.Ast.Declarations;
using Mlt.Parsing;
using Xunit;

namespace Mlt.Parsing.UnitTests;

public class ParserEpic1Tests
{
    [Fact]
    public void Should_Parse_Variable_Declaration_With_Initializer()
    {
        string code = """
        function main(): int {
            var x = 10;
        }
        """;

        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        VariableDeclaration decl = Assert.IsType<VariableDeclaration>(
            program.MainFunction.Body.Nodes.Single()
        );

        Assert.Equal("x", decl.Name);
        LiteralExpression lit = Assert.IsType<LiteralExpression>(decl.Initializer);
        Assert.Equal("10", lit.Value.ToString());
    }

    [Fact]
    public void Should_Parse_Arithmetic_Expression_With_Priorities()
    {
        string code = """
        function main(): int {
            print(2 + 2 * 2);
        }
        """;

        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        PrintStatement print = Assert.IsType<PrintStatement>(
            program.MainFunction.Body.Nodes.Single()
        );

        BinaryOperationExpression addExpr = Assert.IsType<BinaryOperationExpression>(
            print.Arguments.Single()
        );

        Assert.Equal(BinaryOperation.Add, addExpr.Operation);
        Assert.IsType<BinaryOperationExpression>(addExpr.Right);
        Assert.Equal(BinaryOperation.Multiply, ((BinaryOperationExpression)addExpr.Right).Operation);
    }

    [Fact]
    public void Should_Parse_Assignment_As_ExpressionStatement()
    {
        string code = """
        function main(): int {
            var a = 1;
            a = 5;
        }
        """;

        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        // Проверяем второй узел — теперь это честный ExpressionStatement
        ExpressionStatement exprStmt = Assert.IsType<ExpressionStatement>(
            program.MainFunction.Body.Nodes[1]
        );

        AssignmentExpression assign = Assert.IsType<AssignmentExpression>(exprStmt.Expression);
        Assert.Equal("a", ((VariableAccessExpression)assign.Left).Name);
    }

    [Fact]
    public void Should_Parse_Nested_Parentheses()
    {
        string code = """
        function main(): int {
            var z = (1 + (2 * 3));
        }
        """;

        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        VariableDeclaration decl = Assert.IsType<VariableDeclaration>(
            program.MainFunction.Body.Nodes.Single()
        );

        // Проверяем, что парсер "проглотил" глубокую вложенность
        Assert.IsType<BinaryOperationExpression>(decl.Initializer);
    }

    [Fact]
    public void Should_Parse_Multiple_Variables_And_Print()
    {
        string code = """
        function main(): int {
            var a = 10;
            var b = 20;
            print(a + b);
        }
        """;

        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.Equal(3, program.MainFunction.Body.Nodes.Count);
        Assert.IsType<VariableDeclaration>(program.MainFunction.Body.Nodes[0]);
        Assert.IsType<VariableDeclaration>(program.MainFunction.Body.Nodes[1]);
        Assert.IsType<PrintStatement>(program.MainFunction.Body.Nodes[2]);
    }

    [Fact]
    public void Should_Throw_On_Incomplete_Expression()
    {
        string code = """
        function main(): int {
            var x = 10 + ;
        }
        """;

        Parser parser = new Parser(code);

        Assert.ThrowsAny<System.Exception>(() => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_On_Invalid_Variable_Name()
    {
        string code = """
        function main(): int {
            var 1x = 10;
        }
        """;

        Parser parser = new Parser(code);
        Assert.ThrowsAny<System.Exception>(() => parser.ParseProgram());
    }
}