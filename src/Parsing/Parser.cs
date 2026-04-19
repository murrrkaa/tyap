using System;
using System.Collections.Generic;
using System.Globalization;

using Mlt.Ast;
using Mlt.Ast.Declarations;
using Mlt.Ast.Expressions;
using Mlt.Ast.Statements;
using Mlt.Lexemes;
using Mlt.Runtime;
using Mlt.VirtualMachine.Exceptions;

using Expression = Mlt.Ast.Expressions.Expression;
using VmValueType = Mlt.Runtime.ValueType;

namespace Mlt.Parsing;

public class Parser
{
    private readonly TokenStream _tokens;

    public Parser(string code)
    {
        _tokens = new TokenStream(code);
    }

    public Program ParseProgram()
    {
        MainFunctionDeclaration mainFunction = ParseMainFunction();
        Match(TokenType.EndOfFile);
        return new Program(mainFunction);
    }

    private MainFunctionDeclaration ParseMainFunction()
    {
        Match(TokenType.Function);
        Match(TokenType.Main);
        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);
        Match(TokenType.Colon);

        Token returnTypeToken = _tokens.Peek();
        if (returnTypeToken.Type != TokenType.Int)
        {
            throw new ProgramAbortedException($"Ожидался тип 'int' для функции main, но найдено '{returnTypeToken.Type}'");
        }

        Match(TokenType.Int);

        Match(TokenType.OpenBrace);
        BlockStatement body = ParseBlock();
        Match(TokenType.CloseBrace);

        return new MainFunctionDeclaration(body);
    }

    private BlockStatement ParseBlock()
    {
        List<Statement> statements = new List<Statement>();

        while (_tokens.Peek().Type != TokenType.CloseBrace)
        {
            if (_tokens.Peek().Type == TokenType.EndOfFile)
            {
                throw new UnexpectedLexemeException(
                    _tokens.Peek(),
                    TokenType.CloseBrace);
            }

            statements.Add(ParseStatement());
            Match(TokenType.Semicolon);
        }

        return new BlockStatement(statements.AsReadOnly());
    }

    private Statement ParseStatement()
    {
        TokenType currentType = _tokens.Peek().Type;

        if (currentType == TokenType.Print)
        {
            return ParsePrint();
        }

        if (currentType == TokenType.Return)
        {
            return ParseReturn();
        }

        throw new UnexpectedLexemeException(
            _tokens.Peek(),
            new List<TokenType> { TokenType.Print, TokenType.Return });
    }

    private PrintStatement ParsePrint()
    {
        Match(TokenType.Print);
        Match(TokenType.OpenParenthesis);

        List<Expression> args = new List<Expression>();

        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            args.Add(ParseLiteral());

            while (_tokens.Peek().Type == TokenType.Comma)
            {
                Match(TokenType.Comma);
                args.Add(ParseLiteral());
            }
        }

        Match(TokenType.CloseParenthesis);
        return new PrintStatement(args.AsReadOnly());
    }

    private ReturnStatement ParseReturn()
    {
        Match(TokenType.Return);

        if (_tokens.Peek().Type == TokenType.Semicolon)
        {
            return new ReturnStatement(null);
        }

        Expression value = ParseLiteral();
        return new ReturnStatement(value);
    }

    private Expression ParseLiteral()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.IntLiteral:
                _tokens.Advance();
                return new LiteralExpression(
                    VmValueType.Int,
                    new Value(int.Parse(token.Value!.ToString(), CultureInfo.InvariantCulture)));

            case TokenType.FloatLiteral:
                _tokens.Advance();
                return new LiteralExpression(
                    VmValueType.Float,
                    new Value(decimal.Parse(token.Value!.ToString(), CultureInfo.InvariantCulture)));

            case TokenType.StringLiteral:
                _tokens.Advance();
                return new LiteralExpression(
                    VmValueType.String,
                    new Value(token.Value!.ToString()));

            case TokenType.OpenParenthesis:
                _tokens.Advance();
                Expression inner = ParseLiteral();
                Match(TokenType.CloseParenthesis);
                return inner;

            default:
                throw new UnexpectedLexemeException(
                    token,
                    new List<TokenType>
                    {
                        TokenType.IntLiteral,
                        TokenType.FloatLiteral,
                        TokenType.StringLiteral,
                        TokenType.OpenParenthesis,
                    });
        }
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