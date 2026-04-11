using PsTiger.Interpreter;
using PsTiger.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace Interpreter.IntegrationTests;

public class EntryPointTest
{
    [Theory]
    [MemberData(nameof(GetValidEntryPointData))]
    public void Can_execute_main(
        string code,
        string expectedOutput,
        int expectedExitCode)
    {
        FakeEnvironment environment = new();
        TigerInterpreter interpreter = new(environment);

        interpreter.Execute(code);

        Assert.Equal(expectedOutput, environment.BufferedOutput);
        Assert.Equal(expectedExitCode, interpreter.ExitCode);
    }

    public static TheoryData<string, string, int> GetValidEntryPointData()
    {
        return new TheoryData<string, string, int>
        {
            {
                """
                function main(): int 
                {
                    print(42);
                    return 0;
                }
                """,
                "42",
                0
            },
            {
                """
                function main(): int 
                {
                    print(3.14);
                    return 0;
                }
                """,
                "3.14",
                0
            },
            {
                """
                function main(): int 
                {
                    return 0;
                }
                """,
                "",
                0
            },
            {
                """
                function main(): int 
                {
                    print('hello');
                    return 0;
                }
                """,
                "hello",
                0
            },
            {
                """
                function main(): int 
                {
                    return 5;
                }
                """,
                "",
                5
            },
        };
    }
}