using System.Linq;

using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using Mlt.Parsing;
using Mlt.Runtime;

using Xunit;

using VmValueType = Mlt.Runtime.ValueType;

namespace Mlt.Parsing.UnitTests;

public class ParserTest
{
    [Fact]
    public void Should_Parse_Empty_Main()
    {
        string code = """
        function main(): int {
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
        Assert.Empty(program.MainFunction.Body.Nodes);
    }

    [Fact]
    public void Should_Parse_Print_Int()
    {
        string code = """
        function main(): int {
            print(5);
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        LiteralExpression literal =
            Assert.IsType<LiteralExpression>(
                print.Arguments.Single());

        Assert.Equal("5", literal.Value.ToString());
    }

    [Fact]
    public void Should_Parse_Print_String()
    {
        string code = """
        function main(): int {
            print('hello');
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        LiteralExpression literal =
            Assert.IsType<LiteralExpression>(
                print.Arguments.Single());

        Assert.Equal("hello", literal.Value.ToString());
    }

    [Fact]
    public void Should_Parse_Print_Multiple_Arguments()
    {
        string code = """
        function main(): int {
            print(1, 2, 'a');
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.Equal(3, print.Arguments.Count);
    }

    [Fact]
    public void Should_Parse_Return_With_Value()
    {
        string code = """
        function main(): int {
            return 5;
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        ReturnStatement ret =
            Assert.IsType<ReturnStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.NotNull(ret.Expression);

        Assert.IsType<LiteralExpression>(ret.Expression);
    }

    [Fact]
    public void Should_Parse_Return_Without_Value()
    {
        string code = """
        function main(): int {
            return;
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        ReturnStatement ret =
            Assert.IsType<ReturnStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.Null(ret.Expression);
    }

    [Fact]
    public void Should_Parse_Float_Literal()
    {
        string code = """
        function main(): int {
            print(3.14);
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        LiteralExpression literal =
            Assert.IsType<LiteralExpression>(
                print.Arguments.Single());

        Assert.Equal("3.14", literal.Value.ToString());
    }

    [Fact]
    public void Should_Parse_Parenthesized_Literal()
    {
        string code = """
        function main(): int {
            print((5));
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.Single(print.Arguments);
    }

    [Fact]
    public void Should_Parse_Multiple_Statements()
    {
        string code = """
        function main(): int {
            print(1);
            print(2);
            return 0;
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        Assert.Equal(3, program.MainFunction.Body.Nodes.Count);
    }

    [Fact]
    public void Should_Parse_Print_Without_Arguments()
    {
        string code = """
        function main(): int {
            print();
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.Empty(print.Arguments);
    }

    [Fact]
    public void Should_Parse_Expression_Statement()
    {
        string code = """
        function main(): int {
            123;
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        ExpressionStatement statement =
            Assert.IsType<ExpressionStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.IsType<LiteralExpression>(statement.Expression);
    }

    [Fact]
    public void Should_Parse_Variable_Declaration()
    {
        string code = """
        function main(): int {
            var x: int = 5;
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        VariableDeclaration declaration =
            Assert.IsType<VariableDeclaration>(
                program.MainFunction.Body.Nodes.Single());

        Assert.Equal("x", declaration.Name);
        Assert.Equal(VmValueType.Int, declaration.Type);
        Assert.True(declaration.IsMutable);

        Assert.IsType<LiteralExpression>(declaration.Initializer);
    }

    [Fact]
    public void Should_Parse_Const_Declaration()
    {
        string code = """
        function main(): int {
            const pi: float = 3.14;
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        VariableDeclaration declaration =
            Assert.IsType<VariableDeclaration>(
                program.MainFunction.Body.Nodes.Single());

        Assert.False(declaration.IsMutable);
    }

    [Fact]
    public void Should_Parse_Assignment()
    {
        string code = """
        function main(): int {
            x = 5;
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        ExpressionStatement statement =
            Assert.IsType<ExpressionStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.IsType<AssignmentExpression>(statement.Expression);
    }

    [Fact]
    public void Should_Parse_Addition()
    {
        string code = """
        function main(): int {
            print(1 + 2);
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        BinaryOperationExpression expression =
            Assert.IsType<BinaryOperationExpression>(
                print.Arguments.Single());

        Assert.Equal(BinaryOperation.Add, expression.Operation);
    }

    [Fact]
    public void Should_Parse_Multiplication()
    {
        string code = """
        function main(): int {
            print(2 * 3);
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        BinaryOperationExpression expression =
            Assert.IsType<BinaryOperationExpression>(
                print.Arguments.Single());

        Assert.Equal(BinaryOperation.Multiply, expression.Operation);
    }

    [Fact]
    public void Should_Respect_Operator_Precedence()
    {
        string code = """
        function main(): int {
            print(1 + 2 * 3);
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        BinaryOperationExpression add =
            Assert.IsType<BinaryOperationExpression>(
                print.Arguments.Single());

        Assert.Equal(BinaryOperation.Add, add.Operation);

        BinaryOperationExpression multiply =
            Assert.IsType<BinaryOperationExpression>(
                add.Right);

        Assert.Equal(BinaryOperation.Multiply, multiply.Operation);
    }

    [Fact]
    public void Should_Parse_Variable_Access()
    {
        string code = """
        function main(): int {
            print(x);
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.IsType<VariableAccessExpression>(
            print.Arguments.Single());
    }

    [Fact]
    public void Should_Throw_When_Missing_Semicolon()
    {
        string code = """
        function main(): int {
            print(1)
        }
        """;

        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(
            () => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_When_Missing_Close_Brace()
    {
        string code = """
        function main(): int {
            print(1);
        """;

        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(
            () => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_On_Invalid_Print_Syntax()
    {
        string code = """
        function main(): int {
            print(,);
        }
        """;

        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(
            () => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_On_Invalid_Return_Syntax()
    {
        string code = """
        function main(): int {
            return );
        }
        """;

        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(
            () => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_When_Main_Return_Type_Is_Not_Int()
    {
        string code = """
        function main(): float {
        }
        """;

        Parser parser = new(code);

        Assert.Throws<Exception>(
            () => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_When_Variable_Has_No_Initializer()
    {
        string code = """
        function main(): int {
            var x: int;
        }
        """;

        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(
            () => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_When_Missing_Assignment_In_Declaration()
    {
        string code = """
        function main(): int {
            var x: int 5;
        }
        """;

        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(
            () => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_When_Expression_Is_Invalid()
    {
        string code = """
        function main(): int {
            print(1 + );
        }
        """;

        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(
            () => parser.ParseProgram());
    }

    [Fact]
    public void Should_Throw_When_Parenthesis_Is_Not_Closed()
    {
        string code = """
        function main(): int {
            print((1 + 2);
        }
        """;

        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(
            () => parser.ParseProgram());
    }
}