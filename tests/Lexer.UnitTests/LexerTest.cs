using System.Collections.Generic;

using PsTiger.Lexemes;

using Xunit;

namespace PsTiger.Lexemes.UnitTests;

public class LexerTest
{
    [Theory]
    [MemberData(nameof(GetTokenizeIdentifiersAndKeywordsData))]
    [MemberData(nameof(GetTokenizeLiteralsData))]
    [MemberData(nameof(GetSkipWhitespacesAndCommentsData))]
    [MemberData(nameof(GetTokenizeOperatorsAndPunctuationData))]
    public void Can_tokenize_lexemes(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0, iEnd = actual.Count; i < iEnd; ++i)
        {
            Assert.Equivalent(expected[i], actual[i]);
        }
    }

    public static TheoryData<string, List<Token>> GetTokenizeIdentifiersAndKeywordsData()
    {
        return new TheoryData<string, List<Token>>
    {
        // Простые идентификаторы
        {
            "x foo Bar_123 _private",
            [
                new Token(TokenType.Identifier, "x"),
                new Token(TokenType.Identifier, "foo"),
                new Token(TokenType.Identifier, "Bar_123"),
                new Token(TokenType.Identifier, "_private"),
            ]
        },

        // Идентификаторы: регистр имеет значение
        {
            "Var VAR var",
            [
                new Token(TokenType.Identifier, "Var"),
                new Token(TokenType.Identifier, "VAR"),
                new Token(TokenType.Var, "var"),
            ]
        },

        // Ключевые слова: управляющие конструкции
        {
            "if else for while break continue",
            [
                new Token(TokenType.If, "if"),
                new Token(TokenType.Else, "else"),
                new Token(TokenType.For, "for"),
                new Token(TokenType.While, "while"),
                new Token(TokenType.Break, "break"),
                new Token(TokenType.Continue, "continue"),
            ]
        },

        // Ключевые слова: объявления
        {
            "function return var const",
            [
                new Token(TokenType.Function, "function"),
                new Token(TokenType.Return, "return"),
                new Token(TokenType.Var, "var"),
                new Token(TokenType.Const, "const"),
            ]
        },

        // Ключевые слова: типы
        {
            "int float string void bool",
            [
                new Token(TokenType.Int, "int"),
                new Token(TokenType.Float, "float"),
                new Token(TokenType.String, "string"),
                new Token(TokenType.Void, "void"),
                new Token(TokenType.Bool, "bool"),
            ]
        },

        // Ключевые слова: логические
        {
            "and or true false",
            [
                new Token(TokenType.And, "and"),
                new Token(TokenType.Or, "or"),
                new Token(TokenType.True, "true"),
                new Token(TokenType.False, "false"),
            ]
        },

        // Встроенная функция print
        {
            "print(x)",
            [
                new Token(TokenType.Print, "print"),
                new Token(TokenType.OpenParenthesis),
                new Token(TokenType.Identifier, "x"),
                new Token(TokenType.CloseParenthesis),
            ]
        },
    };
    }

    public static TheoryData<string, List<Token>> GetTokenizeLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            // Числа
            {
                "0 42 999999",
                [
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.IntLiteral, 42),
                    new Token(TokenType.IntLiteral, 999999),
                ]
            },
            {
                "3.14 0.5 100.0",
                [
                    new Token(TokenType.FloatLiteral, 3.14),
                    new Token(TokenType.FloatLiteral, 0.5),
                    new Token(TokenType.FloatLiteral, 100.0),
                ]
            },

            // Строки
            {
                "'' 'hello' '123'",
                [
                    new Token(TokenType.StringLiteral, ""),
                    new Token(TokenType.StringLiteral, "hello"),
                    new Token(TokenType.StringLiteral, "123"),
                ]
            },
            {
                "'path\\\\to' 'it\\'s ok'",
                [
                    new Token(TokenType.StringLiteral, "path\\to"),
                    new Token(TokenType.StringLiteral, "it's ok"),
                ]
            },
            {
                "'line1\\nline2'",
                [
                    new Token(TokenType.Error),
                ]
            },
            {
                "'col1\\tcol2'",
                [
                    new Token(TokenType.Error),
                ]
            },
            {
                "'hello\nworld'",
                [
                    new Token(TokenType.Error),
                ]
            },
            {
                "'hello",
                [
                    new Token(TokenType.Error),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetSkipWhitespacesAndCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            // Пропуск пробельных символов
            {
                "x \t\r\n  y",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Identifier, "y"),
                ]
            },

            // Однострочный комментарий #
            {
                "x = 1; # this is a comment",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.Semicolon),
                ]
            },

            // Комментарий # в начале строки
            {
                "# just a comment",
                []
            },

            // Многострочный комментарий /* */
            {
                "/* comment */ a",
                [
                    new Token(TokenType.Identifier, "a"),
                ]
            },

            // Многострочный комментарий с кодом внутри
            {
                "a /* start /* nested */ end */ b",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Identifier, "b"),
                ]
            },

            // Смешанные комментарии
            {
                "var x = 1; # single\n/* multi\nline */ var y = 2;",
                [
                    new Token(TokenType.Var, "var"),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 1),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Var, "var"),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 2),
                    new Token(TokenType.Semicolon),
                ]
            },

            // Вложенные комментарии
            {
                "/* outer /* inner */ still outer */ code",
                [
                    new Token(TokenType.Identifier, "code"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeOperatorsAndPunctuationData()
    {
        return new TheoryData<string, List<Token>>
        {
            // Арифметические операторы
            {
                "a + b - c * d / e",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.Identifier, "d"),
                    new Token(TokenType.Divide),
                    new Token(TokenType.Identifier, "e"),
                ]
            },

            // Операторы сравнения
            {
                "x == y != z < a <= b > c >= d",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Equal),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.Identifier, "z"),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.Identifier, "d"),
                ]
            },

            // Логические операторы
            {
                "!a and b or c",
                [
                    new Token(TokenType.Not),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.And, "and"),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Or, "or"),
                    new Token(TokenType.Identifier, "c"),
                ]
            },

            // Присваивание =
            {
                "x = 42",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 42),
                ]
            },

            // Разделители: скобки, запятая, точка с запятой
            {
                "f(a, b); { x; }",
                [
                    new Token(TokenType.Identifier, "f"),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.CloseBrace),
                ]
            },

            // Сложное выражение с приоритетами
            {
                "a + b * c - d / e",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, "d"),
                    new Token(TokenType.Divide),
                    new Token(TokenType.Identifier, "e"),
                ]
            },

            // Логическое выражение
            {
                "x > 0 and y < 10 or !z",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.And, "and"),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.IntLiteral, 10),
                    new Token(TokenType.Or, "or"),
                    new Token(TokenType.Not),
                    new Token(TokenType.Identifier, "z"),
                ]
            },

            // Ошибка: неизвестный символ
            {
                "x @ y",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Error, "@"),
                    new Token(TokenType.Identifier, "y"),
                ]
            },

            // EndOfFile токен
            {
                "",
                []
            },
        };
    }

    private static List<Token> Tokenize(string code)
    {
        List<Token> results = [];
        Lexer lexer = new(code);
        for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}