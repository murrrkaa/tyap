using Mlt.Interpreter;
using Mlt.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace Interpreter.IntegrationTests;

public class BuiltinFunctionsTest
{
    [Fact]
    public void Print_Without_Arguments_Writes_Nothing()
    {
        string code = """
        function main(): int
        {
            print();
            return 0;
        }
        """;

        FakeEnvironment environment = new();
        MltInterpreter interpreter = new(environment);

        interpreter.Execute(code);

        Assert.Equal(string.Empty, environment.BufferedOutput);
    }

    [Fact]
    public void Multiple_Print_Calls_Work()
    {
        string code = """
        function main(): int
        {
            print(1);
            print(2);
            print('abc');
            return 0;
        }
        """;

        FakeEnvironment environment = new();
        MltInterpreter interpreter = new(environment);

        interpreter.Execute(code);

        Assert.Equal("12abc", environment.BufferedOutput);
    }
}