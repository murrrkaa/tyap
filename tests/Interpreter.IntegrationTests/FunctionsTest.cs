using System;

using Mlt.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace Mlt.Interpreter.IntegrationTests;

public class FunctionsTest
{
    [Fact]
    public void Should_Call_User_Function()
    {
        string code = """
        function add(a: int, b: int): int {
            return a + b;
        }

        function main(): int {
            print(add(2, 3));
            return 0;
        }
        """;

        Assert.Equal("5", Run(code));
    }

    [Fact]
    public void Function_Without_Parameters_Should_Work()
    {
        string code = """
        function getValue(): int {
            return 42;
        }

        function main(): int {
            print(getValue());
            return 0;
        }
        """;

        Assert.Equal("42", Run(code));
    }

    [Fact]
    public void Function_With_One_Parameter_Should_Work()
    {
        string code = """
        function square(x: int): int {
            return x * x;
        }

        function main(): int {
            print(square(5));
            return 0;
        }
        """;

        Assert.Equal("25", Run(code));
    }

    [Fact]
    public void Function_Result_Should_Be_Usable_In_Expression()
    {
        string code = """
        function square(x: int): int {
            return x * x;
        }

        function main(): int {
            print(square(5) + 1);
            return 0;
        }
        """;

        Assert.Equal("26", Run(code));
    }

    [Fact]
    public void Nested_Function_Calls_Should_Work()
    {
        string code = """
        function add(a: int, b: int): int {
            return a + b;
        }

        function main(): int {
            print(add(add(1, 2), 3));
            return 0;
        }
        """;

        Assert.Equal("6", Run(code));
    }

    [Fact]
    public void Function_Can_Be_Called_Multiple_Times()
    {
        string code = """
        function increment(x: int): int {
            return x + 1;
        }

        function main(): int {
            print(increment(1));
            print(increment(5));
            print(increment(10));
            return 0;
        }
        """;

        Assert.Equal("2611", Run(code));
    }

    [Fact]
    public void Function_Can_Use_Local_Variables()
    {
        string code = """
        function calc(a: int, b: int): int {
            var result: int = a + b;
            return result;
        }

        function main(): int {
            print(calc(4, 6));
            return 0;
        }
        """;

        Assert.Equal("10", Run(code));
    }

    [Fact]
    public void Function_Returning_Bool_Should_Work()
    {
        string code = """
        function isPositive(x: int): bool {
            return x > 0;
        }

        function main(): int {
            print(isPositive(5));
            print(isPositive(0));
            return 0;
        }
        """;

        Assert.Equal("truefalse", Run(code));
    }

    [Fact]
    public void Len_Returns_String_Length()
    {
        string code = """
        function main(): int {
            print(len('hello'));
            return 0;
        }
        """;

        Assert.Equal("5", Run(code));
    }

    [Fact]
    public void Len_Returns_Zero_For_Empty_String()
    {
        string code = """
        function main(): int {
            print(len(''));
            return 0;
        }
        """;

        Assert.Equal("0", Run(code));
    }

    [Fact]
    public void Substring_Returns_Correct_Part()
    {
        string code = """
        function main(): int {
            print(substring('hello', 1, 3));
            return 0;
        }
        """;

        Assert.Equal("ell", Run(code));
    }

    [Fact]
    public void False_And_Function_Should_Not_Invoke_Function()
    {
        string code = """
        function fail(): bool {
            print('called');
            return true;
        }

        function main(): int {
            print(false and fail());
            return 0;
        }
        """;

        Assert.Equal("false", Run(code));
    }

    [Fact]
    public void True_Or_Function_Should_Not_Invoke_Function()
    {
        string code = """
        function fail(): bool {
            print('called');
            return false;
        }

        function main(): int {
            print(true or fail());
            return 0;
        }
        """;

        Assert.Equal("true", Run(code));
    }

    [Fact]
    public void Function_With_Wrong_Argument_Type_Should_Throw()
    {
        string code = """
        function sum(a: int): int {
        return a;
        }

        function main(): int {
        print(sum('abc'));
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Function_With_Wrong_Argument_Count_Should_Throw()
    {
        string code = """
        function sum(a: int): int {
        return a;
        }

        function main(): int {
        print(sum());
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Function_Without_Return_Should_Throw()
    {
        string code = """
        function test(): int {
        print(1);
        }

        function main(): int {
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Function_Return_Without_Value_Should_Throw()
    {
        string code = """
        function test(): int {
        return;
        }

        function main(): int {
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Function_Returning_Wrong_Type_Should_Throw()
    {
        string code = """
        function test(): int {
        return 'hello';
        }

        function main(): int {
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Void_Function_Returning_Value_Should_Throw()
    {
        string code = """
        function test(): void {
        return 5;
        }

        function main(): int {
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Len_With_Wrong_Argument_Count_Should_Throw()
    {
        string code = """
        function main(): int {
        print(len());
        return 0;
        }
        """;

        Assert.ThrowsAny<Exception>(() => Run(code));
    }

    [Fact]
    public void Len_With_Wrong_Type_Should_Throw()
    {
        string code = """
        function main(): int {
        print(len(123));
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