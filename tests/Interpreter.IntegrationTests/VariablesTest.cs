using Mlt.Interpreter;
using Mlt.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace Mlt.Interpreter.IntegrationTests;

public class VariablesTest
{
    [Fact]
    public void Variable_Should_Be_Stored_And_Loaded()
    {
        string code = """
        function main(): int {
            var x: int = 10;
            print(x);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("10", output);
    }

    [Fact]
    public void Variable_Should_Be_Reassigned()
    {
        string code = """
        function main(): int {
            var x: int = 5;
            x = 20;
            print(x);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("20", output);
    }

    [Fact]
    public void Const_Should_Be_Evaluated()
    {
        string code = """
        function main(): int {
            const x: int = 7;
            print(x);
            return 0;
        }
        """;

        string output = Run(code);

        Assert.Equal("7", output);
    }

    private static string Run(string code)
    {
        FakeEnvironment env = new();
        new MltInterpreter(env).Execute(code);
        env.Flush();
        return env.FlushedOutput.Trim();
    }
}