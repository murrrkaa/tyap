using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Parsing;
using PsTiger.Runtime;

using Xunit;

using TigerValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Tests.Parsing;

public class ParserTests
{
    [Fact]
    public void Test1_ParseIntLiteral()
    {
        // ✅ Синтаксис ПО спецификации: с двоеточиями
        string code = "function main(): int { var x: int = 123; return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
    }

    [Fact]
    public void Test2_ParseVariableAccess()
    {
        string code = "function main(): int { var x: int = myVar; return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test3_ParseBinaryOperation()
    {
        string code = "function main(): int { var x: int = a + b * c; return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test4_ParseAssignmentStatement()
    {
        string code = "function main(): int { x = 5; return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test5_ParseVariableDeclaration()
    {
        // ✅ var name: type = value (с двоеточием)
        string code = "function main(): int { var x: int = 10; return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
    }

    [Fact]
    public void Test6_ParseIfStatement()
    {
        // ✅ if (condition) { ... } else { ... }
        string code = @"
            function main(): int {
                if (x > 0) {
                    print(x);
                } else {
                    print(0);
                }
                return 0;
            }
        ";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test7_ParseForStatement()
    {
        // ✅ for (init; condition; step) { ... }
        string code = @"
            function main(): int {
                for (i = 0; i < 10; i = i + 1) {
                    print(i);
                }
                return 0;
            }
        ";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test8_ParseFunctionDeclaration()
    {
        // ✅ function name(param: type, ...): return_type { ... }
        string code = @"
            function add(a: int, b: int): int {
                return a + b;
            }
            function main(): int {
                var x: int = add(5, 3);
                return 0;
            }
        ";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
    }

    [Fact]
    public void Test9_ParseFullProgram()
    {
        string code = @"
            function main(): int {
                var x: int = 5;
                var y: int = 10;
                print(x + y);
                return 0;
            }
        ";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
        Assert.Equal("main", program.MainFunction.Name);
    }

    [Fact]
    public void Test10_ParseError_UnexpectedToken()
    {
        string code = "function main(): int { x +; return 0; }";
        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    // === НОВЫЕ ТЕСТЫ для проверки спецификации ===

    [Fact]
    public void Test11_ParseVariableDeclaration_WithColon()
    {
        // ✅ Обязательно двоеточие перед типом
        string code = "function main(): int { var x: int = 42; return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test12_ParseConstantDeclaration_WithColon()
    {
        // ✅ const name: type = value (с двоеточием)
        string code = "function main(): int { const PI: float = 3.14; return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test13_ParseFunctionParameter_WithColon()
    {
        // ✅ Параметры функции: name: type
        string code = "function foo(x: int, y: string): void { } function main(): int { return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test14_ParseFunctionReturnType_Void()
    {
        // ✅ Поддержка : void как типа возврата
        string code = "function helper(): void { } function main(): int { return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test15_ParseFunctionReturnType_Int()
    {
        // ✅ main обязательно возвращает int
        string code = "function main(): int { return 42; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.Equal("int", program.MainFunction.DeclaredReturnTypeName);
    }

    [Fact]
    public void Test16_ParseBuiltInFunction_ReadInt()
    {
        // ✅ Встроенная функция readInt()
        string code = "function main(): int { var x: int = readInt(); return 0; }";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test17_ParseBuiltInFunction_Len()
    {
        // ✅ Встроенная функция len(s)
        string code = @"
            function main(): int {
                var s: string = 'hello';
                var n: int = len(s);
                return 0;
            }
        ";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test18_ParseBuiltInFunction_Substring()
    {
        // ✅ Встроенная функция substring(s, start, count)
        string code = @"
            function main(): int {
                var s: string = substring('hello', 0, 2);
                return 0;
            }
        ";
        Parser parser = new(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test19_ParseError_MissingColonInVar()
    {
        // ❌ Ошибка: отсутствует двоеточие в объявлении переменной
        string code = "function main(): int { var x int = 5; return 0; }";
        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Test20_ParseError_MissingColonInFunctionParam()
    {
        // ❌ Ошибка: отсутствует двоеточие в параметре функции
        string code = "function foo(x int): int { return x; } function main(): int { return 0; }";
        Parser parser = new(code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }
}