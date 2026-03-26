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
        // ✅ Синтаксис БЕЗ двоеточий
        string code = "function main() int { var x int = 123; return 0; }";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
    }

    [Fact]
    public void Test2_ParseVariableAccess()
    {
        string code = "function main() int { var x int = myVar; return 0; }";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test3_ParseBinaryOperation()
    {
        string code = "function main() int { var x int = a + b * c; return 0; }";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test4_ParseAssignmentStatement()
    {
        string code = "function main() int { x = 5; return 0; }";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test5_ParseVariableDeclaration()
    {
        // ✅ var name type = value (без двоеточия)
        string code = "function main() int { var x int = 10; return 0; }";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
    }

    [Fact]
    public void Test6_ParseIfStatement()
    {
        // ✅ if (condition) { ... } else { ... }
        string code = @"
            function main() int {
                if (x > 0) {
                    print(x);
                } else {
                    print(0);
                }
                return 0;
            }
        ";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test7_ParseForStatement()
    {
        // ✅ for (init; condition; step) { ... }
        string code = @"
            function main() int {
                for (i = 0; i < 10; i = i + 1) {
                    print(i);
                }
                return 0;
            }
        ";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
    }

    [Fact]
    public void Test8_ParseFunctionDeclaration()
    {
        // ✅ function name(param type, ...) return_type { ... }
        string code = @"
            function add(a int, b int) int {
                return a + b;
            }
            function main() int {
                var x int = add(5, 3);
                return 0;
            }
        ";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
    }

    [Fact]
    public void Test9_ParseFullProgram()
    {
        string code = @"
            function main() int {
                var x int = 5;
                var y int = 10;
                print(x + y);
                return 0;
            }
        ";
        Parser parser = new Parser(code);
        Program program = parser.ParseProgram();

        Assert.NotNull(program);
        Assert.NotNull(program.MainFunction);
        Assert.Equal("main", program.MainFunction.Name);
    }

    [Fact]
    public void Test10_ParseError_UnexpectedToken()
    {
        string code = "function main() int { x +; return 0; }";
        Parser parser = new Parser(code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }
}