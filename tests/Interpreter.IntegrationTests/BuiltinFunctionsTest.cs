using PsTiger.Interpreter;
using PsTiger.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace Interpreter.IntegrationTests;

public class BuiltinFunctionsTest
{
    [Theory]
    [MemberData(nameof(GetEvaluateBuiltinFunctionsData))]
    public void Can_evaluate_builtin_functions(string code, string expectedOutput)
    {
        FakeEnvironment environment = new();
        TigerInterpreter interpreter = new(environment);

        interpreter.Execute(code);

        Assert.Equal(expectedOutput, environment.BufferedOutput);
    }

    public static TheoryData<string, string> GetEvaluateBuiltinFunctionsData()
    {
        return new TheoryData<string, string>
        {
            // len
            {
                """
                function main(): int {
                    print(len('hello'));
                    return 0;
                }
                """,
                "5"
            },

            // substring
            {
                """
                function main(): int {
                    print(substring('hello', 1, 3));
                    return 0;
                }
                """,
                "ell"
            },

            // toString
            {
                """
                function main(): int {
                    print(toString(123));
                    return 0;
                }
                """,
                "123"
            },

            // parseInt
            {
                """
                function main(): int {
                    print(parseInt('42'));
                    return 0;
                }
                """,
                "42"
            },

            // toBool
            {
                """
                function main(): int {
                    print(toBool(0));
                    return 0;
                }
                """,
                "false"
            },
            {
                """
                function main(): int {
                    print(toBool(5));
                    return 0;
                }
                """,
                "true"
            },

            // toFloat
            {
                """
                function main(): int {
                    print(toFloat(10));
                    return 0;
                }
                """,
                "10"
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetEvaluateInputFunctionsData))]
    public void Can_evaluate_input_functions(
        string code,
        string input,
        string expectedOutput)
    {
        FakeEnvironment environment = new();
        environment.AddInput(input);

        TigerInterpreter interpreter = new(environment);
        interpreter.Execute(code);

        Assert.Equal(expectedOutput, environment.BufferedOutput);
    }

    public static TheoryData<string, string, string> GetEvaluateInputFunctionsData()
    {
        return new TheoryData<string, string, string>
        {
            // readString
            {
                """
                function main(): int {
                    print(readString());
                    return 0;
                }
                """,
                "abc",
                "abc"
            },
        };
    }
}