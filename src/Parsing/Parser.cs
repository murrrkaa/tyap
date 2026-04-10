using System.Globalization;
using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Lexemes;
using PsTiger.Parsing;
using PsTiger.Runtime;
using Expression = PsTiger.Ast.Expressions.Expression;
using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Parsing;

public class Parser
{
    private readonly TokenStream _tokens;

    public Parser(string code)
    {
        _tokens = new TokenStream(code);
    }

    public Program ParseProgram()
    {
        FunctionDeclaration mainFunction = ParseMainFunction();
        Match(TokenType.EndOfFile);
        return new Program(new List<Declaration>().AsReadOnly(), mainFunction);
    }

    private FunctionDeclaration ParseMainFunction()
    {
        Match(TokenType.Function);

        string name = Match(TokenType.Identifier).Value!.ToString();
        if (name != "main")
        {
            throw new UnexpectedLexemeException(
                _tokens.Peek(),
                "Expected identifier 'main' as entry point"
            );
        }

        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);

        Match(TokenType.Colon);
        ValueType returnType = ParseType();
        if (returnType != ValueType.Int)
        {
            throw new UnexpectedLexemeException(
                _tokens.Peek(),
                "Main function must return type 'int'"
            );
        }

        Match(TokenType.OpenBrace);
        BlockStatement body = ParseMainBlock();
        Match(TokenType.CloseBrace);

        return new FunctionDeclaration(
            name,
            new List<ParameterDeclaration>().AsReadOnly(),
            "int",
            ValueType.Int,
            body);
    }

    private BlockStatement ParseMainBlock()
    {
        List<Statement> statements = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace)
        {
            statements.Add(ParseSimpleStatement());
            Match(TokenType.Semicolon);
        }

        return new BlockStatement(statements.AsReadOnly());
    }

    private Statement ParseSimpleStatement()
    {
        switch (_tokens.Peek().Type)
        {
            case TokenType.Print:
                return ParsePrintStatement();

            case TokenType.Return:
                return ParseReturnStatement();

            default:
                throw new UnexpectedLexemeException(
                    _tokens.Peek(),
                    [TokenType.Print, TokenType.Return]
                );
        }
    }

    private PrintStatement ParsePrintStatement()
    {
        Match(TokenType.Print);
        Match(TokenType.OpenParenthesis);

        List<Expression> arguments = [];
        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments.Add(ParseLiteralExpression());
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                arguments.Add(ParseLiteralExpression());
            }
        }

        Match(TokenType.CloseParenthesis);
        return new PrintStatement(arguments.AsReadOnly());
    }

    private ReturnStatement ParseReturnStatement()
    {
        Match(TokenType.Return);
        Expression? value = ParseLiteralExpression();
        return new ReturnStatement(value);
    }

    private Expression ParseLiteralExpression()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.IntLiteral:
                _tokens.Advance();
                return new LiteralExpression(
                    ValueType.Int,
                    new Value(int.Parse(token.Value!.ToString())));

            case TokenType.FloatLiteral:
                _tokens.Advance();
                return new LiteralExpression(
                    ValueType.Float,
                    new Value(double.Parse(token.Value!.ToString(), CultureInfo.InvariantCulture)));

            case TokenType.StringLiteral:
                _tokens.Advance();
                string strValue = UnescapeString(token.Value!.ToString());
                return new LiteralExpression(
                    ValueType.String,
                    new Value(strValue));

            case TokenType.OpenParenthesis:
                _tokens.Advance();
                Expression inner = ParseLiteralExpression();
                Match(TokenType.CloseParenthesis);
                return inner;

            default:
                throw new UnexpectedLexemeException(
                    token,
                    [
                        TokenType.IntLiteral,
                        TokenType.FloatLiteral,
                        TokenType.StringLiteral,
                        TokenType.OpenParenthesis
                    ]
                );
        }
    }

    private ValueType ParseType()
    {
        Token token = _tokens.Peek();
        ValueType type = token.Type switch
        {
            TokenType.Int => ValueType.Int,
            TokenType.Float => ValueType.Float,
            TokenType.String => ValueType.String,
            _ => throw new UnexpectedLexemeException(
                token,
                [TokenType.Int, TokenType.Float, TokenType.String]
            ),
        };

        _tokens.Advance();
        return type;
    }

    private static string UnescapeString(string value)
    {
        return value.Replace("\\\\", "\\").Replace("\\'", "'");
    }

    private Token Match(TokenType expected)
    {
        Token token = _tokens.Peek();
        if (token.Type != expected)
        {
            throw new UnexpectedLexemeException(token, expected);
        }

        _tokens.Advance();
        return token;
    }
}