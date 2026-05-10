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

        if (_tokens.Peek().Type != TokenType.Int)
        {
            _tokens.Advance();
            throw new ProgramAbortedException("Ожидался тип 'int' для функции main");
        }

        Match(TokenType.Int);
        Match(TokenType.OpenBrace);

        BlockStatement body = ParseBlock();

        Match(TokenType.CloseBrace);

        return new MainFunctionDeclaration(body);
    }

    private BlockStatement ParseBlock()
    {
        List<AstNode> nodes = new List<AstNode>();

        while (_tokens.Peek().Type != TokenType.CloseBrace && _tokens.Peek().Type != TokenType.EndOfFile)
        {
            Statement? statement = ParseStatement();

            if (statement != null)
            {
                nodes.Add(statement);
            }
        }

        return new BlockStatement(nodes);
    }

    private Statement? ParseStatement()
    {
        TokenType currentType = _tokens.Peek().Type;

        if (currentType == TokenType.Print)
        {
            return ParsePrint();
        }

        if (currentType == TokenType.Var || currentType == TokenType.Const)
        {
            return ParseVariableDeclaration();
        }

        if (currentType == TokenType.Return)
        {
            return ParseReturn();
        }

        if (currentType == TokenType.CloseBrace)
        {
            return null;
        }

        Expression expr = ParseExpression();

        Match(TokenType.Semicolon);

        return new ExpressionStatement(expr);
    }

    private VariableDeclaration ParseVariableDeclaration()
    {
        Token declToken = _tokens.Peek();
        if (declToken.Type == TokenType.Var)
        {
            Match(TokenType.Var);
        }
        else
        {
            Match(TokenType.Const);
        }

        Token nameToken = _tokens.Peek();
        string name = nameToken.Value?.ToString() ?? throw new Exception("Имя переменной не может быть пустым");
        Match(TokenType.Identifier);

        Match(TokenType.Colon);

        Token typeToken = _tokens.Peek();
        string typeName = typeToken.Value?.ToString() ?? throw new Exception("Тип переменной не указан");

        if (typeToken.Type == TokenType.Int || typeToken.Type == TokenType.Float || typeToken.Type == TokenType.String)
        {
            _tokens.Advance();
        }
        else
        {
            throw new UnexpectedLexemeException(typeToken, TokenType.Int);
        }

        VmValueType varType = ParseType(typeName);

        Match(TokenType.Assignment);
        Expression initializer = ParseExpression();

        Match(TokenType.Semicolon);

        return new VariableDeclaration(
            name,
            varType,
            initializer,
            declToken.Type == TokenType.Var
        );
    }

    private VmValueType ParseType(string typeName)
    {
        return typeName switch
        {
            "int" => VmValueType.Int,
            "float" => VmValueType.Float,
            "string" => VmValueType.String,
            _ => throw new Exception($"Unknown type: {typeName}"),
        };
    }

    private PrintStatement ParsePrint()
    {
        Match(TokenType.Print);
        Match(TokenType.OpenParenthesis);

        List<Expression> args = new List<Expression>();
        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            args.Add(ParseExpression());

            while (_tokens.Peek().Type == TokenType.Comma)
            {
                Match(TokenType.Comma);
                args.Add(ParseExpression());
            }
        }

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);

        return new PrintStatement(args.AsReadOnly());
    }

    private ReturnStatement ParseReturn()
    {
        Match(TokenType.Return);

        if (_tokens.Peek().Type == TokenType.Semicolon)
        {
            Match(TokenType.Semicolon);
            return new ReturnStatement(null);
        }

        Expression value = ParseExpression();
        Match(TokenType.Semicolon);
        return new ReturnStatement(value);
    }

    private Expression ParseExpression()
    {
        return ParseAssignment();
    }

    private Expression ParseAssignment()
    {
        Expression left = ParseAdditive();

        if (_tokens.Peek().Type == TokenType.Assignment)
        {
            Match(TokenType.Assignment);
            Expression right = ParseAssignment();
            return new AssignmentExpression(left, right);
        }

        return left;
    }

    private Expression ParseAdditive()
    {
        Expression left = ParseMultiplicative();

        while (_tokens.Peek().Type == TokenType.Plus || _tokens.Peek().Type == TokenType.Minus)
        {
            TokenType opType = _tokens.Peek().Type;
            _tokens.Advance();

            BinaryOperation op = opType == TokenType.Plus ? BinaryOperation.Add : BinaryOperation.Subtract;
            Expression right = ParseMultiplicative();
            left = new BinaryOperationExpression(left, op, right);
        }

        return left;
    }

    private Expression ParseMultiplicative()
    {
        Expression left = ParsePrimary();

        while (_tokens.Peek().Type == TokenType.Star || _tokens.Peek().Type == TokenType.Slash)
        {
            TokenType opType = _tokens.Peek().Type;
            _tokens.Advance();

            BinaryOperation op = opType == TokenType.Star ? BinaryOperation.Multiply : BinaryOperation.Divide;
            Expression right = ParsePrimary();
            left = new BinaryOperationExpression(left, op, right);
        }

        return left;
    }

    private Expression ParsePrimary()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.IntLiteral:
                _tokens.Advance();
                return new LiteralExpression(VmValueType.Int, new Value(decimal.Parse(token.Value!.ToString()!, CultureInfo.InvariantCulture)));

            case TokenType.FloatLiteral:
                _tokens.Advance();
                return new LiteralExpression(VmValueType.Float, new Value(decimal.Parse(token.Value!.ToString()!, CultureInfo.InvariantCulture)));

            case TokenType.StringLiteral:
                _tokens.Advance();
                return new LiteralExpression(VmValueType.String, new Value(token.Value!.ToString()!));

            case TokenType.Identifier:
                _tokens.Advance();
                return new VariableAccessExpression(token.Value!.ToString()!);

            case TokenType.OpenParenthesis:
                Match(TokenType.OpenParenthesis);
                Expression inner = ParseExpression();
                Match(TokenType.CloseParenthesis);
                return inner;

            default:
                throw new UnexpectedLexemeException(token, TokenType.IntLiteral);
        }
    }

    private void Match(TokenType expected)
    {
        Token token = _tokens.Peek();

        if (token.Type != expected)
        {
            throw new UnexpectedLexemeException(token, expected);
        }

        _tokens.Advance();
    }
}