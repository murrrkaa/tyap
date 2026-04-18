using Mlt.Interpreter;
using Mlt.Semantics.Exceptions;
using Mlt.Tests.TestLibrary.TestDoubles;

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
        MltInterpreter interpreter = new(environment);

        int exitCode = interpreter.Execute(code);

        Assert.Equal(expectedOutput, environment.BufferedOutput);
        Assert.Equal(expectedExitCode, exitCode);
    }

    public static TheoryData<string, string, int> GetValidEntryPointData()
    {
        return new TheoryData<string, string, int>
        {
            // Print int literal
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
            // Print float literal
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
            // Return without print
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
            // Print string literal
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
            // Return non-zero exit code
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

    [Fact]
    public void Main_With_Wrong_ReturnType_Throws_Parse_Error()
    {
        string code = """
        function main(): float 
        {
            return 3.14;
        }
        """;

        FakeEnvironment environment = new();
        MltInterpreter interpreter = new(environment);

        Assert.Throws<Mlt.VirtualMachine.Exceptions.ProgramAbortedException>(() => interpreter.Execute(code));
    }

}