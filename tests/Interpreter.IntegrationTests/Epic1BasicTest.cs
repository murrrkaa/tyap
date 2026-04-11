using PsTiger.Interpreter;
using PsTiger.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace Interpreter.IntegrationTests;

public class Epic1BasicTest
{
    [Theory]
    [MemberData(nameof(GetEpic1TestData))]
    public void Can_execute_basic_program(
        string code,
        string expectedOutput,
        int expectedExitCode)
    {
        FakeEnvironment environment = new();
        TigerInterpreter interpreter = new(environment);

        int exitCode = interpreter.Execute(code);

        Assert.Equal(expectedOutput, environment.BufferedOutput);
        Assert.Equal(expectedExitCode, exitCode);
    }

    public static TheoryData<string, string, int> GetEpic1TestData()
    {
        return new TheoryData<string, string, int>
        {
            // Print int literal
            {
                """
                function main(): int {
                    print(42);
                    return 0;
                }
                """,
                "42",
                0
            },

            // Print float literal
            {
                """
                function main(): int {
                    print(3.14);
                    return 0;
                }
                """,
                "3.14",
                0
            },

            // Print string literal
            {
                """
                function main(): int {
                    print('hello');
                    return 0;
                }
                """,
                "hello",
                0
            },

            // Return without print
            {
                """
                function main(): int {
                    return 0;
                }
                """,
                "",
                0
            },

            // Parenthesized literal
            {
                """
                function main(): int {
                    print((123));
                    return 0;
                }
                """,
                "123",
                0
            },
        };
    }
}