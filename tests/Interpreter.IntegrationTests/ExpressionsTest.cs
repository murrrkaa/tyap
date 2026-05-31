using Mlt.Interpreter;
using Mlt.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace Mlt.Interpreter.IntegrationTests;

public class ExpressionsTest
{
    [Fact]
    public void Should_Add_Ints()
    {
        string code = """
        function main(): int {
            print(2 + 3);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("5", output);
    }

    [Fact]
    public void Should_Subtract_Ints()
    {
        string code = """
        function main(): int {
            print(10 - 4);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("6", output);
    }

    [Fact]
    public void Should_Multiply_Ints()
    {
        string code = """
        function main(): int {
            print(6 * 7);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("42", output);
    }

    [Fact]
    public void Should_Divide_Ints()
    {
        string code = """
        function main(): int {
            print(20 / 5);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("4", output);
    }

    [Fact]
    public void Should_Add_Floats()
    {
        string code = """
        function main(): int {
            print(1.5 + 2.5);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("4", output);
    }

    [Fact]
    public void Should_Subtract_Floats()
    {
        string code = """
        function main(): int {
            print(10.5 - 0.5);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("10", output);
    }

    [Fact]
    public void Should_Multiply_Floats()
    {
        string code = """
        function main(): int {
            print(2.0 * 3.5);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("7", output);
    }

    [Fact]
    public void Should_Divide_Floats()
    {
        string code = """
        function main(): int {
            print(7.5 / 2.5);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("3", output);
    }

    [Fact]
    public void Should_Respect_Operator_Precedence()
    {
        string code = """
        function main(): int {
            print(2 + 3 * 4);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("14", output);
    }

    [Fact]
    public void Should_Respect_Parentheses()
    {
        string code = """
        function main(): int {
            print((2 + 3) * 4);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("20", output);
    }

    [Fact]
    public void Should_Assign_To_Variable()
    {
        string code = """
        function main(): int {
            var x: int = 5;
            x = 10;
            print(x);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("10", output);
    }

    [Fact]
    public void Should_Use_Variable_In_Expression()
    {
        string code = """
        function main(): int {
            var x: int = 5;
            var y: int = 10;
            print(x + y);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("15", output);
    }

    [Fact]
    public void Should_Evaluate_Complex_Expression()
    {
        string code = """
        function main(): int {
            var x: int = 2;
            var y: int = 3;
            print((x + y) * 4 - 6 / 2);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("17", output);
    }

    [Fact]
    public void Should_Print_Multiple_Expressions()
    {
        string code = """
        function main(): int {
            print(1 + 1, 2 * 3, 8 - 5);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("263", output);
    }

    [Fact]
    public void Should_Concatenate_Strings()
    {
        string code = """
        function main(): int {
            print('hello' + ' world');
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("hello world", output);
    }

    [Fact]
    public void Should_Assign_Expression_Result()
    {
        string code = """
        function main(): int {
            var x: int = 1 + 2 * 3;
            print(x);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("7", output);
    }

    [Fact]
    public void Equal_Should_Work()
    {
        string code = """
        function main(): int {
            print(5 == 5);
            return 0;
        }
        """;

        Assert.Equal("true", Run(code));
    }

    [Fact]
    public void NotEqual_Should_Work()
    {
        string code = """
        function main(): int {
            print(5 != 3);
            return 0;
        }
        """;

        Assert.Equal("true", Run(code));
    }

    [Fact]
    public void Less_And_Greater_Should_Work()
    {
        string code = """
        function main(): int {
            print(3 < 5);
            print(5 > 3);
            return 0;
        }
        """;

        Assert.Equal("truetrue", Run(code));
    }

    [Fact]
    public void And_Should_Work()
    {
        string code = """
        function main(): int {
            print(true and true);
            print(true and false);
            return 0;
        }
        """;

        Assert.Equal("truefalse", Run(code));
    }

    [Fact]
    public void Or_Should_Work()
    {
        string code = """
        function main(): int {
            print(true or false);
            print(false or false);
            return 0;
        }
        """;

        Assert.Equal("truefalse", Run(code));
    }

    [Fact]
    public void Not_Should_Work()
    {
        string code = """
        function main(): int {
            print(!false);
            return 0;
        }
        """;

        Assert.Equal("true", Run(code));
    }

    [Fact]
    public void LessOrEqual_Should_Work()
    {
        string code = """
        function main(): int {
        print(5 <= 5);
        print(4 <= 5);
        return 0;
        }
        """;

        Assert.Equal("truetrue", Run(code));
    }

    [Fact]
    public void GreaterOrEqual_Should_Work()
    {
        string code = """
        function main(): int {
        print(5 >= 5);
        print(6 >= 5);
        return 0;
        }
        """;

        Assert.Equal("truetrue", Run(code));
    }

    [Fact]
    public void And_With_True_And_False_Should_Return_False()
    {
        string code = """
        function main(): int {
        print(true and false);
        return 0;
        }
        """;

        Assert.Equal("false", Run(code));
    }

    [Fact]
    public void Or_With_True_And_False_Should_Return_True()
    {
        string code = """
        function main(): int {
        print(true or false);
        return 0;
        }
        """;

        Assert.Equal("true", Run(code));
    }

    [Fact]
    public void Not_True_Should_Return_False()
    {
        string code = """
        function main(): int {
        print(!true);
        return 0;
        }
        """;

        Assert.Equal("false", Run(code));
    }

    [Fact]
    public void Equal_With_Incompatible_Types_Should_Throw()
    {
        string code = """
        function main(): int {
        print(5 == 'hello');
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void And_With_Left_Not_Bool_Should_Throw()
    {
        string code = """
        function main(): int {
        print(5 and true);
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void And_With_Right_Not_Bool_Should_Throw()
    {
        string code = """
        function main(): int {
        print(true and 5);
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Bool_Addition_Should_Throw()
    {
        string code = """
        function main(): int {
        print(true + false);
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Unary_Not_With_Int_Should_Throw()
    {
        string code = """
        function main(): int {
        print(!5);
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Add_With_Incompatible_Types_Should_Throw()
    {
        string code = """
        function main(): int {
        print(1 + true);
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void String_Subtraction_Should_Throw()
    {
        string code = """
        function main(): int {
        print('a' - 'b');
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void LessThan_For_Bool_Should_Throw()
    {
        string code = """
        function main(): int {
        print(true < false);
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    private static string Run(string code)
    {
        FakeEnvironment environment = new();

        MltInterpreter interpreter = new(environment);

        interpreter.Execute(code);

        environment.Flush();

        return environment.FlushedOutput.Trim();
    }
}