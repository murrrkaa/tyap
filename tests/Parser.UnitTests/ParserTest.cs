using System.Linq;

using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using Mlt.Parsing;
using Mlt.Runtime;
using Mlt.VirtualMachine.Exceptions;

using Xunit;

using VmValueType = Mlt.Runtime.ValueType;

namespace Mlt.Parsing.UnitTests;

public class ParserTest
{
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

        Assert.NotNull(declaration.InitialValue);
        Assert.IsType<LiteralExpression>(declaration.InitialValue);
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

        ConstantDeclaration declaration = Assert.IsType<ConstantDeclaration>(
            program.MainFunction.Body.Nodes.Single());

        Assert.Equal("pi", declaration.Name);
        Assert.NotNull(declaration.InitialValue);
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

        AssignmentStatement statement =
            Assert.IsType<AssignmentStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.Equal("x", statement.VariableName);
        Assert.IsType<LiteralExpression>(statement.Value);
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

        Assert.Throws<ProgramAbortedException>(
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

    [Fact]
    public void Should_Parse_GreaterThan_Comparisons()
    {
        string code = """
        function main(): int {
            print(5 > 3, 5 >= 3);
        }
        """;

        Parser parser = new(code);
        Program program = parser.ParseProgram();

        PrintStatement print = Assert.IsType<PrintStatement>(
            program.MainFunction.Body.Nodes.Single());

        Assert.Equal(2, print.Arguments.Count);

        BinaryOperationExpression gt = Assert.IsType<BinaryOperationExpression>(print.Arguments[0]);
        Assert.Equal(BinaryOperation.GreaterThan, gt.Operation);

        BinaryOperationExpression gte = Assert.IsType<BinaryOperationExpression>(print.Arguments[1]);
        Assert.Equal(BinaryOperation.GreaterThanOrEqual, gte.Operation);
    }

    [Fact]
    public void Should_Parse_Logical_And_Or()
    {
        string code = """
        function main(): int {
            print(true and false or true);
        }
        """;

        Parser parser = new(code);
        Program program = parser.ParseProgram();

        PrintStatement print = Assert.IsType<PrintStatement>(program.MainFunction.Body.Nodes.Single());

        BinaryOperationExpression orExpr = Assert.IsType<BinaryOperationExpression>(print.Arguments.Single());
        Assert.Equal(BinaryOperation.Or, orExpr.Operation);

        BinaryOperationExpression andExpr = Assert.IsType<BinaryOperationExpression>(orExpr.Left);
        Assert.Equal(BinaryOperation.And, andExpr.Operation);
    }

    [Fact]
    public void Should_Parse_True_Literal()
    {
        string code = """
        function main(): int {
            print(true);
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

        Assert.Equal(VmValueType.Bool, literal.Type);
    }

    [Fact]
    public void Should_Parse_False_Literal()
    {
        string code = """
        function main(): int {
            print(false);
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

        Assert.Equal(VmValueType.Bool, literal.Type);
    }

    [Fact]
    public void Should_Parse_Function_Call_Statement()
    {
        string code = """
        function main(): int {
            foo();
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        FunctionCallStatement statement =
            Assert.IsType<FunctionCallStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.Equal("foo", statement.Call.Name);
        Assert.Empty(statement.Call.Arguments);
    }

    [Fact]
    public void Should_Parse_Function_Call_With_Arguments()
    {
        string code = """
        function main(): int {
            foo(1, true, 'abc');
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        FunctionCallStatement statement =
            Assert.IsType<FunctionCallStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.Equal(3, statement.Call.Arguments.Count);
    }

    [Fact]
    public void Should_Parse_Function_Call_Expression_Inside_Print()
    {
        string code = """
        function main(): int {
            print(sum(1, 2));
        }
        """;

        Parser parser = new(code);

        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        FunctionCallExpression expression =
            Assert.IsType<FunctionCallExpression>(
                print.Arguments.Single());

        Assert.Equal("sum", expression.Name);
        Assert.Equal(2, expression.Arguments.Count);
    }

    [Fact]
    public void Should_Parse_Subtraction()
    {
        string code = """
        function main(): int {
            print(10 - 5);
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

        Assert.Equal(BinaryOperation.Subtract, expression.Operation);
    }

    [Fact]
    public void Should_Parse_Division()
    {
        string code = """
        function main(): int {
            print(10 / 2);
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

        Assert.Equal(BinaryOperation.Divide, expression.Operation);
    }

    [Fact]
    public void Should_Parse_Equal_Comparison()
    {
        string code = """
        function main(): int {
            print(5 == 5);
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

        Assert.Equal(BinaryOperation.Equal, expression.Operation);
    }

    [Fact]
    public void Should_Parse_NotEqual_Comparison()
    {
        string code = """
        function main(): int {
            print(5 != 3);
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

        Assert.Equal(BinaryOperation.NotEqual, expression.Operation);
    }

    [Fact]
    public void Should_Parse_LessThan_Comparisons()
    {
        string code = """
        function main(): int {
            print(1 < 2, 1 <= 2);
        }
        """;

        Parser parser = new(code);
        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        BinaryOperationExpression lt =
            Assert.IsType<BinaryOperationExpression>(
                print.Arguments[0]);

        Assert.Equal(BinaryOperation.LessThan, lt.Operation);

        BinaryOperationExpression lte =
            Assert.IsType<BinaryOperationExpression>(
                print.Arguments[1]);

        Assert.Equal(BinaryOperation.LessThanOrEqual, lte.Operation);
    }

    [Fact]
    public void Should_Parse_Unary_Not()
    {
        string code = """
        function main(): int {
            print(!true);
        }
        """;

        Parser parser = new(code);
        Program program = parser.ParseProgram();

        PrintStatement print =
            Assert.IsType<PrintStatement>(
                program.MainFunction.Body.Nodes.Single());

        Assert.IsType<UnaryNotExpression>(
            print.Arguments.Single());
    }

    [Fact]
    public void Should_Parse_Function_Without_Parameters()
    {
        string code = """
        function foo(): void {
        }

        function main(): int {
        }
        """;

        Parser parser = new(code);
        Program program = parser.ParseProgram();

        FunctionDeclaration function =
            Assert.IsType<FunctionDeclaration>(
                program.TopLevelStatements.Single());

        Assert.Empty(function.Parameters);
    }
}