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

    [Theory]
    [MemberData(nameof(GetInvalidEntryPointData))]
    public void Throws_on_invalid_entry_point(string code)
    {
        FakeEnvironment environment = new();
        MltInterpreter interpreter = new(environment);

        Assert.ThrowsAny<Exception>(() => interpreter.Execute(code));
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

    public static TheoryData<string> GetInvalidEntryPointData()
    {
        return new TheoryData<string>
        {
            """
            function test(): int 
            {
                return 0;
            }
            """,

            """
            function main(): int 
            {
                print(1);
            }
            """,

            """
            function main(): int 
            {
                return 'hello';
            }
            """,

            """
            function main(): string 
            {
                return 'hello';
            }
            """,
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