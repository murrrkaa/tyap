using System.Collections.Generic;

using Mlt.Lexemes;

using Xunit;

namespace Mlt.Lexemes.UnitTests;

public class LexerTest
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
        List<Token> actual = new List<Token>();
        Lexer lexer = new Lexer(code);

        // Заменили var на Token
        for (Token token = lexer.ParseToken(); token.Type != TokenType.EndOfFile; token = lexer.ParseToken())
        {
            actual.Add(token);
        }

        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < actual.Count; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

    public static TheoryData<string, List<Token>> GetIdentifiersAndKeywordsData() => new TheoryData<string, List<Token>>
    {
        {
            "foo _bar A123 xyz",
            new List<Token>
            {
                new Token(TokenType.Identifier, "foo"),
                new Token(TokenType.Identifier, "_bar"),
                new Token(TokenType.Identifier, "A123"),
                new Token(TokenType.Identifier, "xyz"),
            }
        },
        {
            "function var const main int float string return print",
            new List<Token>
            {
                new Token(TokenType.Function, "function"),
                new Token(TokenType.Var, "var"),
                new Token(TokenType.Const, "const"),
                new Token(TokenType.Main, "main"),
                new Token(TokenType.Int, "int"),
                new Token(TokenType.Float, "float"),
                new Token(TokenType.String, "string"),
                new Token(TokenType.Return, "return"),
                new Token(TokenType.Print, "print"),
            }
        },
    };

    public static TheoryData<string, List<Token>> GetNumberLiteralsData() => new TheoryData<string, List<Token>>
    {
        {
            "0 1234",
            new List<Token>
            {
                // Используем decimal (M), так как Lexer теперь работает с ним
                new Token(TokenType.IntLiteral, 0m),
                new Token(TokenType.IntLiteral, 1234m),
            }
        },
        {
            "0.5 123.456",
            new List<Token>
            {
                new Token(TokenType.FloatLiteral, 0.5m),
                new Token(TokenType.FloatLiteral, 123.456m),
            }
        },
        {
            "123.",
            new List<Token>
            {
                new Token(TokenType.IntLiteral, 123m),
                new Token(TokenType.Error, "Unexpected character '.'"),
            }
        }
    };

    public static TheoryData<string, List<Token>> GetStringLiteralsData() => new TheoryData<string, List<Token>>
    {
        {
            "'' 'Hello'",
            new List<Token>
            {
                new Token(TokenType.StringLiteral, ""),
                new Token(TokenType.StringLiteral, "Hello"),
            }
        },
        {
            "'\\\\'",
            new List<Token>
            {
                new Token(TokenType.StringLiteral, "\\"),
            }
        }
    };

    public static TheoryData<string, List<Token>> GetPunctuationData() => new TheoryData<string, List<Token>>
    {
        {
            "{ } ( ) : , ; =",
            new List<Token>
            {
                new Token(TokenType.OpenBrace),
                new Token(TokenType.CloseBrace),
                new Token(TokenType.OpenParenthesis),
                new Token(TokenType.CloseParenthesis),
                new Token(TokenType.Colon),
                new Token(TokenType.Comma),
                new Token(TokenType.Semicolon),
                new Token(TokenType.Assignment),
            }
        }
    };

    public static TheoryData<string, List<Token>> GetCommentsAndWhitespaceData() => new TheoryData<string, List<Token>>
    {
        {
            "x # comment\n y",
            new List<Token>
            {
                new Token(TokenType.Identifier, "x"),
                new Token(TokenType.Identifier, "y"),
            }
        },
        {
            "a /* comment */ b",
            new List<Token>
            {
                new Token(TokenType.Identifier, "a"),
                new Token(TokenType.Identifier, "b"),
            }
        }
    };

    public static TheoryData<string, List<Token>> GetErrorCasesData() => new TheoryData<string, List<Token>>
    {
        {
            "@",
            new List<Token>
            {
                new Token(TokenType.Error, "Unexpected character '@'"),
            }
        }
    };
}