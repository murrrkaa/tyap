using Xunit;
using System.Collections.Generic;
using PsTiger.Lexemes;

public class LexerTests
{
    [Theory]
    [MemberData(nameof(GetIdentifiersAndKeywordsData))]
    [MemberData(nameof(GetNumberLiteralsData))]
    [MemberData(nameof(GetStringLiteralsData))]
    [MemberData(nameof(GetPunctuationData))]
    [MemberData(nameof(GetCommentsAndWhitespaceData))]
    [MemberData(nameof(GetErrorCasesData))]
    public void Tokenize_ReturnsExpectedTokens(string code, List<Token> expected)
    {
        // Выполняем лексический разбор входа
        List<Token> actual = new List<Token>();
        Lexer lexer = new Lexer(code);
        for (Token token = lexer.ParseToken(); token.Type != TokenType.EndOfFile; token = lexer.ParseToken())
            actual.Add(token);

        // Сравниваем количество и сами токены
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < actual.Count; i++)
            Assert.Equal(expected[i], actual[i]);
    }

    public static TheoryData<string, List<Token>> GetIdentifiersAndKeywordsData() => new TheoryData<string, List<Token>>
    {
        // Идентификаторы: буквы, цифры, _
        { "foo _bar A123 xyz",
            new List<Token> {
                new Token(TokenType.Identifier, "foo"),
                new Token(TokenType.Identifier, "_bar"),
                new Token(TokenType.Identifier, "A123"),
                new Token(TokenType.Identifier, "xyz")
            }
        },
        // Ключевые слова vs похожие имена
        { "function foo main int float string return print Func Int str",
            new List<Token> {
                new Token(TokenType.Function, "function"),
                new Token(TokenType.Identifier, "foo"),
                new Token(TokenType.Main, "main"),
                new Token(TokenType.Int, "int"),
                new Token(TokenType.Float, "float"),
                new Token(TokenType.String, "string"),
                new Token(TokenType.Return, "return"),
                new Token(TokenType.Print, "print"),
                new Token(TokenType.Identifier, "Func"),  // чувствительность к регистру
                new Token(TokenType.Identifier, "Int"),
                new Token(TokenType.Identifier, "str")
            }
        },
    };

    public static TheoryData<string, List<Token>> GetNumberLiteralsData() => new TheoryData<string, List<Token>>
    {
        // Целые и ведущие нули
        { "0 1234 0017",
            new List<Token> {
                new Token(TokenType.IntLiteral, 0),
                new Token(TokenType.IntLiteral, 1234),
                new Token(TokenType.IntLiteral, 17)
            }
        },
        // Дробные числа
        { "0.5 10.0 123.456",
            new List<Token> {
                new Token(TokenType.FloatLiteral, 0.5),
                new Token(TokenType.FloatLiteral, 10.0),
                new Token(TokenType.FloatLiteral, 123.456)
            }
        },
        // Число с точкой, но без цифр после (точка становится отдельным символом-ошибкой)
        { "123.",
            new List<Token> {
                new Token(TokenType.IntLiteral, 123),
                new Token(TokenType.Error, "Unexpected character '.'")
            }
        },
        // Неправильный формат (не цифра после точки)
        { "45.a 678",
            new List<Token> {
                new Token(TokenType.IntLiteral, 45),
                new Token(TokenType.Error, "Unexpected character 'a'"),
                new Token(TokenType.IntLiteral, 678)
            }
        },
        // Слишком большое целое → ошибка парсинга
        { "999999999999999999999999",
            new List<Token> {
                new Token(TokenType.Error, "Invalid integer literal: '999999999999999999999999'")
            }
        },
    };

    public static TheoryData<string, List<Token>> GetStringLiteralsData() => new TheoryData<string, List<Token>>
    {
        // Пустая и простая строки
        { "'' '0' 'Hello, world!'",
            new List<Token> {
                new Token(TokenType.StringLiteral, ""),
                new Token(TokenType.StringLiteral, "0"),
                new Token(TokenType.StringLiteral, "Hello, world!")
            }
        },
        // Простые экранирования
        { "'\\\\' '\\' '",
            new List<Token> {
                new Token(TokenType.StringLiteral, "\\"),
                new Token(TokenType.StringLiteral, "'")
            }
        },
        // Нераспознанное экранирование
        { "'\\a'",
            new List<Token> {
                new Token(TokenType.Error, "Invalid escape sequence")
            }
        },
        // Unterminated string (нет закрывающей кавычки)
        { "'abc",
            new List<Token> {
                new Token(TokenType.Error, "Unterminated string literal")
            }
        },
        // Перевод строки внутри строки
        { "'line\nbreak'",
            new List<Token> {
                new Token(TokenType.Error, "Unterminated string literal")
            }
        },
    };

    public static TheoryData<string, List<Token>> GetPunctuationData() => new TheoryData<string, List<Token>>
    {
        { "{ } ( ) : , ;",
            new List<Token> {
                new Token(TokenType.OpenBrace),
                new Token(TokenType.CloseBrace),
                new Token(TokenType.OpenParenthesis),
                new Token(TokenType.CloseParenthesis),
                new Token(TokenType.Colon),
                new Token(TokenType.Comma),
                new Token(TokenType.Semicolon)
            }
        },
        // Пример кода с пунктуацией и числами
        { "{ main() : return 0; }",
            new List<Token> {
                new Token(TokenType.OpenBrace),
                new Token(TokenType.Main, "main"),
                new Token(TokenType.OpenParenthesis),
                new Token(TokenType.CloseParenthesis),
                new Token(TokenType.Colon),
                new Token(TokenType.Return, "return"),
                new Token(TokenType.IntLiteral, 0),
                new Token(TokenType.Semicolon),
                new Token(TokenType.CloseBrace)
            }
        },
    };

    public static TheoryData<string, List<Token>> GetCommentsAndWhitespaceData() => new TheoryData<string, List<Token>>
    {
        // Пропуск пробелов и табуляций
        { " x\t\ty\n",
            new List<Token> {
                new Token(TokenType.Identifier, "x"),
                new Token(TokenType.Identifier, "y")
            }
        },
        // Однострочный комментарий
        { "foo # comment\n bar",
            new List<Token> {
                new Token(TokenType.Identifier, "foo"),
                new Token(TokenType.Identifier, "bar")
            }
        },
        // Многострочный комментарий
        { "a /* mid-comment */ b",
            new List<Token> {
                new Token(TokenType.Identifier, "a"),
                new Token(TokenType.Identifier, "b")
            }
        },
        // Не закрытый многострочный комментарий до конца входа
        { "x /* open comment",
            new List<Token> {
                new Token(TokenType.Identifier, "x")
                // После комментария — конец файла
            }
        },
    };

    public static TheoryData<string, List<Token>> GetErrorCasesData() => new TheoryData<string, List<Token>>
    {
        // Неожиданный символ
        { "@", new List<Token> { new Token(TokenType.Error, "Unexpected character '@'") } },
        // Неправильный символ для литерала (например '$' не обрабатывается)
        { "$100", new List<Token> { new Token(TokenType.Error, "Unexpected character '$'"), new Token(TokenType.IntLiteral, 100) } },
        // Неподдерживаемый символ внутри строки
        { "'abc\\z'", new List<Token> { new Token(TokenType.Error, "Invalid escape sequence") } },
        // Заглавные буквы в ключевом слове (в данном лексере считается идентификатором)
        { "Return", new List<Token> { new Token(TokenType.Identifier, "Return") } },
    };
}
