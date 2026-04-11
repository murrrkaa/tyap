//using PsTiger.Interpreter;
//using PsTiger.Tests.TestLibrary.TestDoubles;

//using Xunit;

//namespace Interpreter.IntegrationTests;

//public class EntryPointTest
//{
//    [Theory]
//    [MemberData(nameof(GetValidEntryPointData))]
//    public void Can_execute_main(
//        string code,
//        string expectedOutput,
//        int expectedExitCode)
//    {
//        FakeEnvironment environment = new();
//        TigerInterpreter interpreter = new(environment);

//        interpreter.Execute(code);

//        Assert.Equal(expectedOutput, environment.BufferedOutput);
//        Assert.Equal(expectedExitCode, interpreter.ExitCode);
//    }

//    [Theory]
//    [MemberData(nameof(GetInvalidEntryPointData))]
//    public void Throws_on_invalid_entry_point(string code)
//    {
//        FakeEnvironment environment = new();
//        TigerInterpreter interpreter = new(environment);

//        Assert.ThrowsAny<Exception>(() => interpreter.Execute(code));
//    }

//    public static TheoryData<string, string, int> GetValidEntryPointData()
//    {
//        return new TheoryData<string, string, int>
//        {
//            // пустая программа
//            {
//                """
//                function main(): int {
//                    return 0;
//                }
//                """,
//                "",
//                0
//            },

//            // вывод
//            {
//                """
//                function main(): int {
//                    print('hello');
//                    return 0;
//                }
//                """,
//                "hello",
//                0
//            },

//            // возврат ненулевого exit code
//            {
//                """
//                function main(): int {
//                    return 5;
//                }
//                """,
//                "",
//                5
//            },

//            // выражение + вывод
//            {
//                """
//                function main(): int {
//                    print(1 + 2);
//                    return 0;
//                }
//                """,
//                "3",
//                0
//            },
//        };
//    }

//    public static TheoryData<string> GetInvalidEntryPointData()
//    {
//        return new TheoryData<string>
//        {
//            // нет main
//            """
//            function test(): int {
//                return 0;
//            }
//            """,

//            // main без return
//            """
//            function main(): int {
//                print(1);
//            }
//            """,

//            // неправильный тип return
//            """
//            function main(): int {
//                return 'hello';
//            }
//            """,

//            // несколько функций (если у вас запрещено)
//            """
//            function main(): int {
//                return 0;
//            }

//            function other(): int {
//                return 0;
//            }
//            """,

//            // неверный тип main
//            """
//            function main(): string {
//                return 'hello';
//            }
//            """,
//        };
//    }
//}