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
    [MemberData(nameof(GetOperatorsAndPunctuationData))]
    [MemberData(nameof(GetCommentsAndWhitespaceData))]
    [MemberData(nameof(GetComparisonAndLogicalOperatorsData))]
    [MemberData(nameof(GetBoolAndVoidKeywordData))]
    [MemberData(nameof(GetErrorCasesData))]
    public void Tokenize_ReturnsExpectedTokens(string code, List<Token> expected)
    {
        List<Token> actual = new List<Token>();
        Lexer lexer = new Lexer(code);

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

    public static TheoryData<string, List<Token>> GetIdentifiersAndKeywordsData()
        => new TheoryData<string, List<Token>>
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
            "main Main MAIN",
            new List<Token>
            {
                new Token(TokenType.Main, "main"),
                new Token(TokenType.Identifier, "Main"),
                new Token(TokenType.Identifier, "MAIN"),
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

    public static TheoryData<string, List<Token>> GetBoolAndVoidKeywordData()
        => new()
    {
        {
            "bool void true false and or",
            new List<Token>
            {
                new(TokenType.Bool, "bool"),
                new(TokenType.Void, "void"),
                new(TokenType.True, "true"),
                new(TokenType.False, "false"),
                new(TokenType.And, "and"),
                new(TokenType.Or, "or"),
            }
        },
    };

    public static TheoryData<string, List<Token>> GetNumberLiteralsData()
        => new TheoryData<string, List<Token>>
    {
        {
            "0 1234 0017",
            new List<Token>
            {
                new Token(TokenType.IntLiteral, 0m),
                new Token(TokenType.IntLiteral, 1234m),
                new Token(TokenType.IntLiteral, 17m),
            }
        },
        {
            "0.5 10.0 123.456",
            new List<Token>
            {
                new Token(TokenType.FloatLiteral, 0.5m),
                new Token(TokenType.FloatLiteral, 10.0m),
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
        },
        {
            "45.a",
            new List<Token>
            {
                new Token(TokenType.IntLiteral, 45m),
                new Token(TokenType.Error, "Unexpected character '.'"),
                new Token(TokenType.Identifier, "a"),
            }
        },
    };

    public static TheoryData<string, List<Token>> GetStringLiteralsData()
        => new TheoryData<string, List<Token>>
    {
        {
            "'' '0' 'Hello, world!'",
            new List<Token>
            {
                new Token(TokenType.StringLiteral, ""),
                new Token(TokenType.StringLiteral, "0"),
                new Token(TokenType.StringLiteral, "Hello, world!"),
            }
        },
        {
            @"'\\' '\'' '\n'",
            new List<Token>
            {
                new Token(TokenType.StringLiteral, "\\"),
                new Token(TokenType.StringLiteral, "'"),
                new Token(TokenType.StringLiteral, "\n"),
            }
        },
        {
            "'!@#$%^&*()_+-='",
            new List<Token>
            {
                new Token(TokenType.StringLiteral, "!@#$%^&*()_+-="),
            }
        },
        {
            "'\\a'",
            new List<Token>
            {
                new Token(TokenType.Error, "Invalid escape sequence"),
            }
        },
        {
            "'abc",
            new List<Token>
            {
                new Token(TokenType.Error, "Unterminated string literal"),
            }
        },
        {
            "'\\\\'",
            new List<Token>
            {
                new Token(TokenType.StringLiteral, "\\"),
            }
        },
    };

    public static TheoryData<string, List<Token>> GetOperatorsAndPunctuationData()
        => new TheoryData<string, List<Token>>
    {
        {
            "{ } ( ) : , ;",
            new List<Token>
            {
                new Token(TokenType.OpenBrace),
                new Token(TokenType.CloseBrace),
                new Token(TokenType.OpenParenthesis),
                new Token(TokenType.CloseParenthesis),
                new Token(TokenType.Colon),
                new Token(TokenType.Comma),
                new Token(TokenType.Semicolon),
            }
        },
        {
            "+ - * /",
            new List<Token>
            {
                new Token(TokenType.Plus),
                new Token(TokenType.Minus),
                new Token(TokenType.Multiply),
                new Token(TokenType.Divide),
            }
        },
        {
            "=",
            new List<Token>
            {
                new Token(TokenType.Assign),
            }
        },
        {
            "{ main() : return 0; }",
            new List<Token>
            {
                new Token(TokenType.OpenBrace),
                new Token(TokenType.Main, "main"),
                new Token(TokenType.OpenParenthesis),
                new Token(TokenType.CloseParenthesis),
                new Token(TokenType.Colon),
                new Token(TokenType.Return, "return"),
                new Token(TokenType.IntLiteral, 0),
                new Token(TokenType.Semicolon),
                new Token(TokenType.CloseBrace),
            }
        },
    };

    public static TheoryData<string, List<Token>> GetComparisonAndLogicalOperatorsData()
        => new()
    {
        {
            "== != < <= > >= !",
            new List<Token>
            {
                new(TokenType.Equal),
                new(TokenType.NotEqual),
                new(TokenType.LessThan),
                new(TokenType.LessThanOrEqual),
                new(TokenType.GreaterThan),
                new(TokenType.GreaterThanOrEqual),
                new(TokenType.Not),
            }
        },
        {
            "a > b and c <= d or !e",
            new List<Token>
            {
                new(TokenType.Identifier, "a"),
                new(TokenType.GreaterThan),
                new(TokenType.Identifier, "b"),
                new(TokenType.And, "and"),
                new(TokenType.Identifier, "c"),
                new(TokenType.LessThanOrEqual),
                new(TokenType.Identifier, "d"),
                new(TokenType.Or, "or"),
                new(TokenType.Not),
                new(TokenType.Identifier, "e"),
            }
        },
    };
    public static TheoryData<string, List<Token>> GetCommentsAndWhitespaceData()
        => new TheoryData<string, List<Token>>
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
            " x\t\ty\n",
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
        },
    };

    public static TheoryData<string, List<Token>> GetErrorCasesData() => new TheoryData<string, List<Token>>
    {
        {
            "@",
            new List<Token>
            {
                new Token(TokenType.Error, "Unexpected character '@'"),
            }
        },
        {
            "$100",
            new List<Token>
            {
                new Token(TokenType.Error, "Unexpected character '$'"),
                new Token(TokenType.IntLiteral, 100),
            }
        },
        {
            "'abc\\z'",
            new List<Token>
            {
                new Token(TokenType.Error, "Invalid escape sequence"),
            }
        },
        {
            "Return",
            new List<Token>
            {
                new Token(TokenType.Identifier, "Return"),
            }
        },
    };
}