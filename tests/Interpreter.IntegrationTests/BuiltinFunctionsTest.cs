using System;

using Mlt.Interpreter;
using Mlt.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace Interpreter.IntegrationTests;

public class BuiltinFunctionsTest
{
    [Fact]
    public void Print_int_literal_outputs_integer()
    {
        string code =
            """
            function main(): int {
                print(123);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("123", environment.BufferedOutput);
    }

    [Fact]
    public void Print_negative_int_expression_outputs_result()
    {
        string code =
            """
            function main(): int {
                print(10 - 15);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("-5", environment.BufferedOutput);
    }

    [Fact]
    public void Print_float_literal_outputs_float()
    {
        string code =
            """
            function main(): int {
                print(12.5);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("12.5", environment.BufferedOutput);
    }

    [Fact]
    public void Print_float_expression_outputs_result()
    {
        string code =
            """
            function main(): int {
                print(10.5 + 1.5);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("12.0", environment.BufferedOutput);
    }

    [Fact]
    public void Print_string_literal_outputs_string()
    {
        string code =
            """
            function main(): int {
                print('Hello');
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("Hello", environment.BufferedOutput);
    }

    [Fact]
    public void Print_string_concatenation_outputs_concatenated_string()
    {
        string code =
            """
            function main(): int {
                print('Hello, ' + 'world!');
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("Hello, world!", environment.BufferedOutput);
    }

    [Fact]
    public void Print_multiple_arguments_outputs_all_values_in_order()
    {
        string code =
            """
            function main(): int {
                print(1, 'abc', 2.5);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("1abc2.5", environment.BufferedOutput);
    }

    [Fact]
    public void Print_variable_outputs_variable_value()
    {
        string code =
            """
            function main(): int {
                var x: int = 42;
                print(x);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("42", environment.BufferedOutput);
    }

    [Fact]
    public void Print_updated_variable_outputs_new_value()
    {
        string code =
            """
            function main(): int {
                var x: int = 10;
                x = 25;
                print(x);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("25", environment.BufferedOutput);
    }

    [Fact]
    public void Print_const_variable_outputs_value()
    {
        string code =
            """
            function main(): int {
                const text: string = 'constant';
                print(text);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("constant", environment.BufferedOutput);
    }

    [Fact]
    public void Print_expression_with_operator_precedence_outputs_correct_result()
    {
        string code =
            """
            function main(): int {
                print(2 + 3 * 4);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("14", environment.BufferedOutput);
    }

    [Fact]
    public void Print_expression_with_parentheses_outputs_correct_result()
    {
        string code =
            """
            function main(): int {
                print((2 + 3) * 4);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("20", environment.BufferedOutput);
    }

    [Fact]
    public void Print_division_of_ints_outputs_integer_division_result()
    {
        string code =
            """
            function main(): int {
                print(7 / 2);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("3", environment.BufferedOutput);
    }

    [Fact]
    public void Print_division_of_floats_outputs_float_division_result()
    {
        string code =
            """
            function main(): int {
                print(7.0 / 2.0);
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("3.5", environment.BufferedOutput);
    }

    [Fact]
    public void Print_escape_sequence_outputs_decoded_string()
    {
        string code =
            """
            function main(): int {
                print('line1\nline2');
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("line1\nline2", environment.BufferedOutput);
    }

    [Fact]
    public void Print_backslash_escape_outputs_backslash()
    {
        string code =
            """
            function main(): int {
                print('\\');
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("\\", environment.BufferedOutput);
    }

    [Fact]
    public void Print_single_quote_escape_outputs_quote()
    {
        string code =
            """
            function main(): int {
                print('\'');
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("'", environment.BufferedOutput);
    }

    [Fact]
    public void Print_after_comments_works_correctly()
    {
        string code =
            """
            function main(): int {
                # single line comment
                /* multi
                   line
                   comment */
                print('ok');
                return 0;
            }
            """;

        FakeEnvironment environment = Execute(code);

        Assert.Equal("ok", environment.BufferedOutput);
    }

    [Fact]
    public void Print_with_type_mismatch_throws_exception()
    {
        string code =
            """
            function main(): int {
                var x: int = 'hello';
                print(x);
                return 0;
            }
            """;

        Action action = () => Execute(code);

        Assert.ThrowsAny<Exception>(() => Execute(code));
    }

    [Fact]
    public void Print_undeclared_variable_throws_exception()
    {
        string code =
            """
            function main(): int {
                print(x);
                return 0;
            }
            """;

        Action action = () => Execute(code);

        Assert.Throws<Exception>(action);
    }

    [Fact]
    public void Print_assignment_to_const_throws_exception()
    {
        string code =
            """
            function main(): int {
                const x: int = 10;
                x = 20;
                print(x);
                return 0;
            }
            """;

        Action action = () => Execute(code);

        Assert.Throws<Exception>(action);
    }

    [Fact]
    public void Print_division_by_zero_throws_exception()
    {
        string code =
            """
            function main(): int {
                print(10 / 0);
                return 0;
            }
            """;

        Action action = () => Execute(code);

        InvalidOperationException ex =
            Assert.Throws<InvalidOperationException>(action);

        Assert.Equal("Division by zero", ex.Message);
    }

    [Fact]
    public void Print_string_subtraction_throws_type_error()
    {
        string code =
            """
            function main(): int {
                print('a' - 'b');
                return 0;
            }
            """;

        Action action = () => Execute(code);

        Assert.ThrowsAny<Exception>(() => Execute(code));
    }

    [Fact]
    public void Main_without_return_throws_exception()
    {
        string code =
            """
            function main(): int {
                print(1);
            }
            """;

        Action action = () => Execute(code);

        Assert.ThrowsAny<Exception>(() => Execute(code));
    }

    [Fact]
    public void Main_returning_non_int_throws_exception()
    {
        string code =
            """
            function main(): int {
                return 'abc';
            }
            """;

        Action action = () => Execute(code);

        Assert.ThrowsAny<Exception>(() => Execute(code));
    }

    private static FakeEnvironment Execute(string code)
    {
        FakeEnvironment environment = new FakeEnvironment();

        MltInterpreter interpreter = new MltInterpreter(environment);

        interpreter.Execute(code);

        return environment;
    }
}