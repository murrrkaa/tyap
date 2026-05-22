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
using ValueType = Mlt.Runtime.ValueType;

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
        List<Declaration> topLevelStatements = [];

        while (!IsMainFunctionNext() && _tokens.Peek().Type != TokenType.EndOfFile)
        {
            topLevelStatements.Add(ParseTopLevelStatement());
        }

        MainFunctionDeclaration mainFunction = ParseMainFunction();

        Match(TokenType.EndOfFile);

        return new Program(topLevelStatements.AsReadOnly(), mainFunction);
    }

    /// <summary>
    /// Разбирает обычное объявление функции.
    /// </summary>
    public FunctionDeclaration ParseFunctionDeclaration()
    {
        Match(TokenType.Function);
        string name = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.OpenParenthesis);
        List<ParameterDeclaration> parameters = ParseTypedParameterList();
        Match(TokenType.CloseParenthesis);

        ValueType returnType = ValueType.Void;
        if (_tokens.Peek().Type == TokenType.Colon)
        {
            _tokens.Advance();
            returnType = ParseReturnType();
        }

        Match(TokenType.OpenBrace);
        BlockStatement body = ParseBlockContent();
        Match(TokenType.CloseBrace);

        return new FunctionDeclaration(
            name,
            parameters.AsReadOnly(),
            returnType == ValueType.Void ? "void" : returnType.ToString().ToLower(),
            returnType,
            body);
    }

    public VariableDeclaration ParseVariableDeclaration()
    {
        Match(TokenType.Var);
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);
        ValueType type = ParseType();
        Match(TokenType.Assign);
        Expression value = ParseExpression();

        return new VariableDeclaration(name, type.ToString().ToLower(), type, value);
    }

    public ConstantDeclaration ParseConstantDeclaration()
    {
        Match(TokenType.Const);
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);
        ValueType type = ParseType();
        Match(TokenType.Assign);
        Expression value = ParseExpression();

        return new ConstantDeclaration(name, type.ToString().ToLower(), type, value);
    }

    /// <summary>
    /// Разбирает выделенную функцию main.
    /// Правило: main_function = "function", "main", "(", ")", ":", "int", "{", {statement}, "}" ;
    /// </summary>
    private MainFunctionDeclaration ParseMainFunction()
    {
        Match(TokenType.Function);
        Match(TokenType.Main); // Используем Main из оригинального TokenType

        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);

        Match(TokenType.Colon);
        ValueType returnType = ParseType();
        if (returnType != ValueType.Int)
        {
            throw new ProgramAbortedException("Семантическое ограничение: Функция main должна возвращать тип 'int'.");
        }

        Match(TokenType.OpenBrace);
        BlockStatement body = ParseBlockContent();
        Match(TokenType.CloseBrace);

        return new MainFunctionDeclaration(body);
    }

    private bool IsMainFunctionNext()
    {
        return _tokens.Peek().Type == TokenType.Function
            && _tokens.Peek(1).Type == TokenType.Main;
    }

    private Declaration ParseTopLevelStatement()
    {
        switch (_tokens.Peek().Type)
        {
            case TokenType.Function:
                return ParseFunctionDeclaration();
            case TokenType.Var:
                VariableDeclaration varDecl = ParseVariableDeclaration();
                Match(TokenType.Semicolon);
                return varDecl;
            case TokenType.Const:
                ConstantDeclaration constDecl = ParseConstantDeclaration();
                Match(TokenType.Semicolon);
                return constDecl;
            default:
                throw new UnexpectedLexemeException(
                    _tokens.Peek(),
                    [TokenType.Function, TokenType.Var, TokenType.Const]
                );
        }
    }

    private List<ParameterDeclaration> ParseTypedParameterList()
    {
        List<ParameterDeclaration> parameters = [];

        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            parameters.Add(ParseTypedParameter());
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                parameters.Add(ParseTypedParameter());
            }
        }

        return parameters;
    }

    private ParameterDeclaration ParseTypedParameter()
    {
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);
        ValueType type = ParseType();

        return new ParameterDeclaration(name, type.ToString().ToLower(), type);
    }

    private ValueType ParseReturnType()
    {
        Token token = _tokens.Peek();
        ValueType returnType = token.Type switch
        {
            TokenType.Int => ValueType.Int,
            TokenType.Float => ValueType.Float,
            TokenType.String => ValueType.String,
            TokenType.Bool => ValueType.Bool,
            TokenType.Void => ValueType.Void,
            _ => throw new UnexpectedLexemeException(
                token,
                [TokenType.Int, TokenType.Float, TokenType.String, TokenType.Bool, TokenType.Void]
            ),
        };

        _tokens.Advance();
        return returnType;
    }

    private ValueType ParseType()
    {
        Token token = _tokens.Peek();
        ValueType type = token.Type switch
        {
            TokenType.Int => ValueType.Int,
            TokenType.Float => ValueType.Float,
            TokenType.String => ValueType.String,
            TokenType.Bool => ValueType.Bool,
            _ => throw new UnexpectedLexemeException(token, [TokenType.Int, TokenType.Float, TokenType.String, TokenType.Bool]),
        };

        _tokens.Advance();
        return type;
    }

    private BlockStatement ParseBlockContent()
    {
        List<Statement> statements = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace && _tokens.Peek().Type != TokenType.EndOfFile)
        {
            statements.Add(ParseStatement());
        }

        return new BlockStatement(statements.AsReadOnly());
    }

    private Statement ParseStatement()
    {
        switch (_tokens.Peek().Type)
        {
            case TokenType.Return:
                return ParseReturnStatement();

            default:
                Statement stmt = ParseSimpleStatement();
                Match(TokenType.Semicolon);
                return stmt;
        }
    }

    private Statement ParseSimpleStatement()
    {
        switch (_tokens.Peek().Type)
        {
            case TokenType.Var:
                return ParseVariableDeclaration();
            case TokenType.Const:
                return ParseConstantDeclaration();
            case TokenType.Print:
                return ParsePrintStatement();
            case TokenType.Identifier:
                if (_tokens.Peek(1).Type == TokenType.Assign)
                {
                    return ParseAssignmentStatement();
                }
                else
                {
                    string name = Match(TokenType.Identifier).Value!.ToString();
                    FunctionCallExpression call = ParseFunctionCall(name);
                    return new FunctionCallStatement(call);
                }

            default:
                throw new UnexpectedLexemeException(
                    _tokens.Peek(),
                    [TokenType.Var, TokenType.Const, TokenType.Print, TokenType.Identifier]
                );
        }
    }

    private AssignmentStatement ParseAssignmentStatement()
    {
        string identifier = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Assign);
        Expression value = ParseExpression();

        return new AssignmentStatement(identifier, value);
    }

    private FunctionCallExpression ParseFunctionCall(string name)
    {
        Match(TokenType.OpenParenthesis);

        List<Expression> arguments = [];
        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments.Add(ParseExpression());
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                arguments.Add(ParseExpression());
            }
        }

        Match(TokenType.CloseParenthesis);

        return new FunctionCallExpression(name, arguments.AsReadOnly());
    }

    private PrintStatement ParsePrintStatement()
    {
        Match(TokenType.Print);
        Match(TokenType.OpenParenthesis);

        List<Expression> arguments = [];
        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments.Add(ParseExpression());
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                arguments.Add(ParseExpression());
            }
        }

        Match(TokenType.CloseParenthesis);

        return new PrintStatement(arguments.AsReadOnly());
    }

    private ReturnStatement ParseReturnStatement()
    {
        Match(TokenType.Return);

        Expression? value = null;
        if (_tokens.Peek().Type != TokenType.Semicolon)
        {
            value = ParseExpression();
        }

        Match(TokenType.Semicolon);

        return new ReturnStatement(value);
    }

    private Expression ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    private Expression ParseLogicalOrExpression()
    {
        Expression expr = ParseLogicalAndExpression();

        while (_tokens.Peek().Type == TokenType.Or)
        {
            _tokens.Advance();
            expr = new BinaryOperationExpression(expr, BinaryOperation.Or, ParseLogicalAndExpression());
        }

        return expr;
    }

    private Expression ParseLogicalAndExpression()
    {
        Expression expr = ParseComparisonExpression();

        while (_tokens.Peek().Type == TokenType.And)
        {
            _tokens.Advance();
            expr = new BinaryOperationExpression(expr, BinaryOperation.And, ParseComparisonExpression());
        }

        return expr;
    }

    private Expression ParseComparisonExpression()
    {
        Expression left = ParseAdditiveExpression();

        TokenType t = _tokens.Peek().Type;
        if (t == TokenType.Equal || t == TokenType.NotEqual ||
            t == TokenType.LessThan || t == TokenType.LessThanOrEqual ||
            t == TokenType.GreaterThan || t == TokenType.GreaterThanOrEqual)
        {
            _tokens.Advance();

            BinaryOperation op = t switch
            {
                TokenType.Equal => BinaryOperation.Equal,
                TokenType.NotEqual => BinaryOperation.NotEqual,
                TokenType.LessThan => BinaryOperation.LessThan,
                TokenType.LessThanOrEqual => BinaryOperation.LessThanOrEqual,
                TokenType.GreaterThan => BinaryOperation.GreaterThan,
                TokenType.GreaterThanOrEqual => BinaryOperation.GreaterThanOrEqual,
                _ => throw new InvalidOperationException(),
            };

            Expression right = ParseAdditiveExpression();
            return new BinaryOperationExpression(left, op, right);
        }

        return left;
    }

    private Expression ParseAdditiveExpression()
    {
        Expression expr = ParseTermExpression();

        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.Plus:
                    _tokens.Advance();
                    expr = new BinaryOperationExpression(expr, BinaryOperation.Add, ParseTermExpression());
                    break;
                case TokenType.Minus:
                    _tokens.Advance();
                    expr = new BinaryOperationExpression(expr, BinaryOperation.Subtract, ParseTermExpression());
                    break;
                default:
                    return expr;
            }
        }
    }

    private Expression ParseTermExpression()
    {
        Expression expr = ParseFactorExpression();

        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.Multiply:
                    _tokens.Advance();
                    expr = new BinaryOperationExpression(expr, BinaryOperation.Multiply, ParseFactorExpression());
                    break;
                case TokenType.Divide:
                    _tokens.Advance();
                    expr = new BinaryOperationExpression(expr, BinaryOperation.Divide, ParseFactorExpression());
                    break;
                default:
                    return expr;
            }
        }
    }

    private Expression ParseFactorExpression()
    {
        if (_tokens.Peek().Type == TokenType.Not)
        {
            _tokens.Advance();
            return new UnaryNotExpression(ParseFactorExpression());
        }

        return ParseSimpleExpression();
    }

    private Expression ParseSimpleExpression()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.IntLiteral:
                _tokens.Advance();
                return new LiteralExpression(ValueType.Int, new Value(long.Parse(token.Value!.ToString()!, CultureInfo.InvariantCulture)));

            case TokenType.FloatLiteral:
                _tokens.Advance();
                return new LiteralExpression(ValueType.Float, new Value(decimal.Parse(token.Value!.ToString()!, CultureInfo.InvariantCulture)));

            case TokenType.StringLiteral:
                _tokens.Advance();
                string strValue = UnescapeString(token.Value!.ToString());
                return new LiteralExpression(ValueType.String, new Value(strValue));

            case TokenType.True:
                _tokens.Advance();
                return new LiteralExpression(ValueType.Bool, new Value(true));

            case TokenType.False:
                _tokens.Advance();
                return new LiteralExpression(ValueType.Bool, new Value(false));

            case TokenType.Identifier:
                string name = token.Value!.ToString();
                _tokens.Advance();
                if (_tokens.Peek().Type == TokenType.OpenParenthesis)
                {
                    return ParseFunctionCall(name);
                }

                return new VariableAccessExpression(name);

            case TokenType.OpenParenthesis:
                _tokens.Advance();
                Expression inner = ParseExpression();
                Match(TokenType.CloseParenthesis);
                return inner;

            default:
                throw new UnexpectedLexemeException(
                    token,
                    [
                        TokenType.IntLiteral,
                        TokenType.FloatLiteral,
                        TokenType.StringLiteral,
                        TokenType.True,
                        TokenType.False,
                        TokenType.Identifier,
                        TokenType.OpenParenthesis
                    ]
                );
        }
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