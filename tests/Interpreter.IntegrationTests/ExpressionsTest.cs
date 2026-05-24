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

    private static string Run(string code)
    {
        FakeEnvironment environment = new();

        MltInterpreter interpreter = new(environment);

        interpreter.Execute(code);

        environment.Flush();

        return environment.FlushedOutput.Trim();
    }
}