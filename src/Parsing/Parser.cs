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

/// <summary>
/// Рекурсивный спуск-парсер для языка по спецификации.
/// </summary>
public class Parser
{
    private readonly TokenStream _tokens;

    public Parser(string code)
    {
        _tokens = new TokenStream(code);
    }

    /// <summary>
    /// Разбирает программу.
    /// Правило: program = { top_level_statement }, main_function ;
    /// </summary>
    public Program ParseProgram()
    {
        List<Declaration> topLevelStatements = [];

        while (!IsMainFunctionNext())
        {
            topLevelStatements.Add(ParseTopLevelStatement());
        }

        FunctionDeclaration mainFunction = ParseMainFunction();

        Match(TokenType.EndOfFile);

        return new Program(topLevelStatements.AsReadOnly(), mainFunction);
    }

    /// <summary>
    /// Разбирает объявление функции.
    /// Правило: function_definition = "function", identifier, "(", [typed_parameter_list], ")", [":", return_type], "{", {function_statement}, "}" ;
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
        BlockStatement body = ParseFunctionBlock();
        Match(TokenType.CloseBrace);

        return new FunctionDeclaration(
            name,
            parameters.AsReadOnly(),
            returnType == ValueType.Void ? "void" : returnType.ToString().ToLower(),
            returnType,
            body);
    }

    /// <summary>
    /// Разбирает объявление переменной.
    /// Правило: variable_declaration = "var", identifier, ":", type, "=", expression ;
    /// </summary>
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

    /// <summary>
    /// Разбирает объявление константы.
    /// Правило: constant_declaration = "const", identifier, ":", type, "=", expression ;
    /// </summary>
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
    /// Проверяет, является ли следующий токен началом функции main.
    /// </summary>
    private bool IsMainFunctionNext()
    {
        return _tokens.Peek().Type == TokenType.Function
            && _tokens.Peek(1).Type == TokenType.Identifier
            && _tokens.Peek(1).Value!.ToString() == "main";
    }

    /// <summary>
    /// Разбирает функцию main.
    /// Правило: main_function = "function", "main", "(", ")", ":", "int", "{", ...
    /// </summary>
    private FunctionDeclaration ParseMainFunction()
    {
        Match(TokenType.Function);
        string name = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);

        Match(TokenType.Colon);
        ValueType returnType = ParseType();
        if (returnType != ValueType.Int)
        {
            throw new UnexpectedLexemeException(
                _tokens.Peek(),
                [TokenType.Int]
            );
        }

        Match(TokenType.OpenBrace);
        BlockStatement body = ParseFunctionBlock();
        Match(TokenType.CloseBrace);

        return new FunctionDeclaration(
            name,
            new List<ParameterDeclaration>().AsReadOnly(),
            returnType.ToString().ToLower(),
            returnType,
            body);
    }

    /// <summary>
    /// Разбирает верхнеуровневую инструкцию.
    /// Правило: top_level_statement = function_definition | variable_declaration ";" | constant_declaration ";" ;
    /// </summary>
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

    /// <summary>
    /// Разбирает список параметров функции.
    /// Правило: typed_parameter_list = typed_parameter, { ",", typed_parameter } ;
    /// </summary>
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

    /// <summary>
    /// Разбирает один параметр функции.
    /// Правило: typed_parameter = identifier, ":", type ;
    /// </summary>
    private ParameterDeclaration ParseTypedParameter()
    {
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Colon);
        ValueType type = ParseType();

        return new ParameterDeclaration(name, type.ToString().ToLower(), type);
    }

    /// <summary>
    /// Разбирает тип возврата функции.
    /// Правило: return_type = type | "void" ;
    /// </summary>
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

    /// <summary>
    /// Разбирает тип данных.
    /// Правило: type = "int" | "float" | "string" | "bool" ;
    /// </summary>
    private ValueType ParseType()
    {
        Token token = _tokens.Peek();
        ValueType type = token.Type switch
        {
            TokenType.Int => ValueType.Int,
            TokenType.Float => ValueType.Float,
            TokenType.String => ValueType.String,
            TokenType.Bool => ValueType.Bool,
            _ => throw new UnexpectedLexemeException(
                token,
                [TokenType.Int, TokenType.Float, TokenType.String, TokenType.Bool]
            ),
        };

        _tokens.Advance();
        return type;
    }

    /// <summary>
    /// Разбирает блок инструкций функции.
    /// </summary>
    private BlockStatement ParseFunctionBlock()
    {
        List<Statement> statements = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace)
        {
            statements.Add(ParseFunctionStatement());
        }

        return new BlockStatement(statements.AsReadOnly());
    }

    /// <summary>
    /// Разбирает инструкцию внутри функции.
    /// </summary>
    private Statement ParseFunctionStatement()
    {
        switch (_tokens.Peek().Type)
        {
            case TokenType.If:
                return ParseIfStatement();
            case TokenType.While:
                return ParseWhileStatement();
            case TokenType.For:
                return ParseForStatement();
            case TokenType.Return:
                return ParseReturnStatement();
            default:
                Statement stmt = ParseSimpleStatement();
                Match(TokenType.Semicolon);
                return stmt;
        }
    }

    /// <summary>
    /// Разбирает простую инструкцию.
    /// </summary>
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
                    FunctionCallExpression call = ParseFunctionCallOrBuiltIn();
                    return new FunctionCallStatement(call);
                }

            default:
                throw new UnexpectedLexemeException(
                    _tokens.Peek(),
                    [TokenType.Var, TokenType.Const, TokenType.Print, TokenType.Identifier]
                );
        }
    }

    /// <summary>
    /// Разбирает присваивание.
    /// </summary>
    private AssignmentStatement ParseAssignmentStatement()
    {
        string identifier = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Assign);
        Expression value = ParseExpression();

        return new AssignmentStatement(identifier, value);
    }

    /// <summary>
    /// Разбирает вызов функции или встроенной функции.
    /// </summary>
    private FunctionCallExpression ParseFunctionCallOrBuiltIn()
    {
        string name = _tokens.Peek().Value!.ToString();
        _tokens.Advance();

        if (_tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            return ParseFunctionCall(name);
        }

        throw new UnexpectedLexemeException(_tokens.Peek(), [TokenType.OpenParenthesis]);
    }

    /// <summary>
    /// Разбирает вызов функции как выражение.
    /// </summary>
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

    /// <summary>
    /// Разбирает инструкцию вывода.
    /// </summary>
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

    /// <summary>
    /// Разбирает if/else.
    /// </summary>
    private IfStatement ParseIfStatement()
    {
        Match(TokenType.If);
        Match(TokenType.OpenParenthesis);
        Expression condition = ParseExpression();
        Match(TokenType.CloseParenthesis);

        BlockStatement thenBlock = ParseBlockStatement();

        BlockStatement? elseBlock = null;
        if (_tokens.Peek().Type == TokenType.Else)
        {
            _tokens.Advance();
            elseBlock = ParseBlockStatement();
        }

        return new IfStatement(condition, thenBlock, elseBlock);
    }

    /// <summary>
    /// Разбирает блок инструкций.
    /// </summary>
    private BlockStatement ParseBlockStatement()
    {
        Match(TokenType.OpenBrace);
        List<Statement> statements = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace)
        {
            statements.Add(ParseStatement());
        }

        Match(TokenType.CloseBrace);

        return new BlockStatement(statements.AsReadOnly());
    }

    /// <summary>
    /// Разбирает инструкцию (внутри блока).
    /// </summary>
    private Statement ParseStatement()
    {
        switch (_tokens.Peek().Type)
        {
            case TokenType.If:
                return ParseIfStatement();
            case TokenType.While:
                return ParseWhileStatement();
            case TokenType.For:
                return ParseForStatement();
            case TokenType.Function:
                return ParseFunctionDeclaration();
            default:
                Statement stmt = ParseSimpleStatement();
                Match(TokenType.Semicolon);
                return stmt;
        }
    }

    /// <summary>
    /// Разбирает while-цикл.
    /// </summary>
    private WhileStatement ParseWhileStatement()
    {
        Match(TokenType.While);
        Match(TokenType.OpenParenthesis);
        Expression condition = ParseExpression();
        Match(TokenType.CloseParenthesis);

        BlockStatement body = ParseLoopBlock();

        return new WhileStatement(condition, body);
    }

    /// <summary>
    /// Разбирает for-цикл.
    /// </summary>
    private ForStatement ParseForStatement()
    {
        Match(TokenType.For);
        Match(TokenType.OpenParenthesis);

        AssignmentStatement initialization = ParseAssignmentStatement();
        Match(TokenType.Semicolon);

        Expression condition = ParseExpression();
        Match(TokenType.Semicolon);

        AssignmentStatement step = ParseAssignmentStatement();

        Match(TokenType.CloseParenthesis);

        BlockStatement body = ParseLoopBlock();

        return new ForStatement(initialization, condition, step, body);
    }

    /// <summary>
    /// Разбирает блок цикла.
    /// </summary>
    private BlockStatement ParseLoopBlock()
    {
        Match(TokenType.OpenBrace);
        List<Statement> statements = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace)
        {
            statements.Add(ParseLoopStatement());
        }

        Match(TokenType.CloseBrace);

        return new BlockStatement(statements.AsReadOnly());
    }

    /// <summary>
    /// Разбирает инструкцию внутри цикла.
    /// </summary>
    private Statement ParseLoopStatement()
    {
        switch (_tokens.Peek().Type)
        {
            case TokenType.Break:
                _tokens.Advance();
                Match(TokenType.Semicolon);
                return new BreakStatement();
            case TokenType.Continue:
                _tokens.Advance();
                Match(TokenType.Semicolon);
                return new ContinueStatement();
            case TokenType.If:
                return ParseIfStatement();
            case TokenType.While:
                return ParseWhileStatement();
            case TokenType.For:
                return ParseForStatement();
            case TokenType.Function:
                return ParseFunctionDeclaration();
            default:
                Statement stmt = ParseSimpleStatement();
                Match(TokenType.Semicolon);
                return stmt;
        }
    }

    /// <summary>
    /// Разбирает return-инструкцию.
    /// </summary>
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

    /// <summary>
    /// Разбирает выражение.
    /// </summary>
    private Expression ParseExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Разбирает логическое ИЛИ.
    /// </summary>
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

    /// <summary>
    /// Разбирает логическое И.
    /// </summary>
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

    /// <summary>
    /// Разбирает операции сравнения.
    /// </summary>
    private Expression ParseComparisonExpression()
    {
        Expression left = ParseAdditiveExpression();

        switch (_tokens.Peek().Type)
        {
            case TokenType.Equal:
                _tokens.Advance();
                return new BinaryOperationExpression(left, BinaryOperation.Equal, ParseAdditiveExpression());
            case TokenType.NotEqual:
                _tokens.Advance();
                return new BinaryOperationExpression(left, BinaryOperation.NotEqual, ParseAdditiveExpression());
            case TokenType.LessThan:
                _tokens.Advance();
                return new BinaryOperationExpression(left, BinaryOperation.LessThan, ParseAdditiveExpression());
            case TokenType.LessThanOrEqual:
                _tokens.Advance();
                return new BinaryOperationExpression(left, BinaryOperation.LessThanOrEqual, ParseAdditiveExpression());
            case TokenType.GreaterThan:
                _tokens.Advance();
                return new BinaryOperationExpression(left, BinaryOperation.GreaterThan, ParseAdditiveExpression());
            case TokenType.GreaterThanOrEqual:
                _tokens.Advance();
                return new BinaryOperationExpression(left, BinaryOperation.GreaterThanOrEqual, ParseAdditiveExpression());
            default:
                return left;
        }
    }

    /// <summary>
    /// Разбирает сложение/вычитание.
    /// </summary>
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

    /// <summary>
    /// Разбирает умножение/деление.
    /// </summary>
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

    /// <summary>
    /// Разбирает унарное НЕ.
    /// </summary>
    private Expression ParseFactorExpression()
    {
        if (_tokens.Peek().Type == TokenType.Not)
        {
            _tokens.Advance();
            return new UnaryNotExpression(ParseFactorExpression());
        }

        return ParseSimpleExpression();
    }

    /// <summary>
    /// Разбирает простое выражение.
    /// </summary>
    private Expression ParseSimpleExpression()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.IntLiteral:
                _tokens.Advance();
                return new LiteralExpression(ValueType.Int, new Value(int.Parse(token.Value!.ToString())));

            case TokenType.FloatLiteral:
                _tokens.Advance();
                return new LiteralExpression(ValueType.Float, new Value(double.Parse(token.Value!.ToString(), CultureInfo.InvariantCulture)));

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

    /// <summary>
    /// Обрабатывает экранированные символы в строке.
    /// </summary>
    private static string UnescapeString(string value)
    {
        return value.Replace("\\\\", "\\").Replace("\\'", "'");
    }

    /// <summary>
    /// Читает ожидаемую лексему либо бросает исключение.
    /// </summary>
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