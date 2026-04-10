using PsTiger.Interpreter;
using PsTiger.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace PsTiger.Interpreter.IntegrationTests;

public class ExpressionsTest
{
    [Theory]
    [MemberData(nameof(GetEvaluateExpressionsData))]
    public void Can_evaluate_expressions(string code, string expectedOutput)
    {
        FakeEnvironment environment = new();
        TigerInterpreter interpreter = new(environment);

        interpreter.Execute(code);

        Assert.Equal(expectedOutput, environment.BufferedOutput);
    }

    [Theory]
    [MemberData(nameof(GetInvalidExpressionsData))]
    public void Rejects_invalid_expressions(string code)
    {
        FakeEnvironment environment = new();
        TigerInterpreter interpreter = new(environment);

        Assert.ThrowsAny<Exception>(() => interpreter.Execute(code));
    }

    public static TheoryData<string, string> GetEvaluateExpressionsData()
    {
        return new TheoryData<string, string>
        {
            // сложение
            {
                """
                function main(): int {
                    print(2 + 3);
                    return 0;
                }
                """,
                "5"
            },

            // вычитание
            {
                """
                function main(): int {
                    print(7 - 2);
                    return 0;
                }
                """,
                "5"
            },

            // умножение
            {
                """
                function main(): int {
                    print(3 * 4);
                    return 0;
                }
                """,
                "12"
            },

            // деление
            {
                """
                function main(): int {
                    print(8 / 2);
                    return 0;
                }
                """,
                "4"
            },

            // приоритет операций
            {
                """
                function main(): int {
                    print(2 + 3 * 4);
                    return 0;
                }
                """,
                "14"
            },

            // скобки
            {
                """
                function main(): int {
                    print((2 + 3) * 4);
                    return 0;
                }
                """,
                "20"
            },

            // вложенные скобки
            {
                """
                function main(): int {
                    print((1 + (2 * 3)));
                    return 0;
                }
                """,
                "7"
            },

            // ассоциативность -
            {
                """
                function main(): int {
                    print(10 - 3 - 2);
                    return 0;
                }
                """,
                "5"
            },

            // ассоциативность /
            {
                """
                function main(): int {
                    print(16 / 4 / 2);
                    return 0;
                }
                """,
                "2"
            },

            // строковое выражение (если разрешено)
            {
                """
                function main(): int {
                    print('Hello ' + 'World');
                    return 0;
                }
                """,
                "Hello World"
            },
        };
    }

    public static TheoryData<string> GetInvalidExpressionsData()
    {
        return new TheoryData<string>
        {
            // незавершенное выражение
            """
            function main(): int {
                print(1 +);
                return 0;
            }
            """,

            // неправильный оператор
            """
            function main(): int {
                print(* 2);
                return 0;
            }
            """,

            // незакрытая скобка
            """
            function main(): int {
                print((1 + 2);
                return 0;
            }
            """,

            // пустые скобки
            """
            function main(): int {
                print(());
                return 0;
            }
            """,
        };
    }
}