using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PsTiger.Lexemes;

public class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "for", TokenType.For },
        { "while", TokenType.While },
        { "function", TokenType.Function },
        { "return", TokenType.Return },
        { "break", TokenType.Break },
        { "continue", TokenType.Continue },
        { "var", TokenType.Var },
        { "const", TokenType.Const },
        { "and", TokenType.And },
        { "or", TokenType.Or },
        { "int", TokenType.Int },
        { "float", TokenType.Float },
        { "string", TokenType.String },
        { "void", TokenType.Void },
        { "bool", TokenType.Bool },
        { "print", TokenType.Print },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "readInt", TokenType.Identifier },
        { "readFloat", TokenType.Identifier },
        { "readString", TokenType.Identifier },
        { "len", TokenType.Identifier },
        { "substring", TokenType.Identifier },
        { "toString", TokenType.Identifier },
        { "parseInt", TokenType.Identifier },
        { "toBool", TokenType.Identifier },
        { "toFloat", TokenType.Identifier },
    };

    private static readonly Dictionary<char, char> SimpleEscapes = new()
    {
        { '\\', '\\' },
        { '\'', '\'' },
    };

    private readonly TextScanner _scanner;

    public Lexer(string code)
    {
        _scanner = new TextScanner(code);
    }

    /// <summary>
    /// Разбирает следующий токен из входного потока.
    /// </summary>
    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile);
        }

        char c = _scanner.Peek();

        if (char.IsAsciiLetter(c) || c == '_')
        {
            return ParseIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(c))
        {
            return ParseNumberLiteral();
        }

        if (c == '\'')
        {
            return ParseStringLiteral();
        }

        switch (c)
        {
            case '+':
                _scanner.Advance();
                return new Token(TokenType.Plus);

            case '-':
                _scanner.Advance();
                return new Token(TokenType.Minus);

            case '*':
                _scanner.Advance();
                return new Token(TokenType.Multiply);

            case '/':
                _scanner.Advance();

                if (_scanner.Peek() == '*')
                {
                    SkipMultiLineComment();
                    return ParseToken();
                }

                return new Token(TokenType.Divide);

            case '=':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Equal);
                }

                return new Token(TokenType.Assign);

            case '<':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LessThanOrEqual);
                }

                return new Token(TokenType.LessThan);

            case '>':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.GreaterThanOrEqual);
                }

                return new Token(TokenType.GreaterThan);

            case '!':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.NotEqual);
                }

                return new Token(TokenType.Not);

            case '{':
                _scanner.Advance();
                return new Token(TokenType.OpenBrace);

            case '}':
                _scanner.Advance();
                return new Token(TokenType.CloseBrace);

            case '(':
                _scanner.Advance();
                return new Token(TokenType.OpenParenthesis);

            case ')':
                _scanner.Advance();
                return new Token(TokenType.CloseParenthesis);

            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);

            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);

            case '#':
                SkipHashComment();
                return ParseToken();
        }

        _scanner.Advance();
        return new Token(TokenType.Error, c.ToString());
    }

    /// <summary>
    /// Разбирает числовой литерал (целое или с плавающей точкой).
    /// number_literal = digit, {digit}, [".", digit, {digit}]
    /// </summary>
    private Token ParseNumberLiteral()
    {
        StringBuilder sb = new();

        while (char.IsAsciiDigit(_scanner.Peek()))
        {
            sb.Append(_scanner.Peek());
            _scanner.Advance();
        }

        if (_scanner.Peek() == '.' && char.IsAsciiDigit(_scanner.Peek(1)))
        {
            sb.Append(_scanner.Peek());
            _scanner.Advance();
            while (char.IsAsciiDigit(_scanner.Peek()))
            {
                sb.Append(_scanner.Peek());
                _scanner.Advance();
            }

            if (double.TryParse(sb.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double floatValue))
            {
                return new Token(TokenType.FloatLiteral, floatValue);
            }

            return new Token(TokenType.Error, sb.ToString());
        }

        if (long.TryParse(sb.ToString(), out long intValue))
        {
            if (intValue >= int.MinValue && intValue <= int.MaxValue)
            {
                return new Token(TokenType.IntLiteral, (int)intValue);
            }

            return new Token(TokenType.IntLiteral, (int)intValue);
        }

        return new Token(TokenType.Error, sb.ToString());
    }

    /// <summary>
    /// Разбирает строковый литерал в одинарных кавычках.
    /// Поддерживает только экранирование: \\ и \'
    /// </summary>
    private Token ParseStringLiteral()
    {
        _scanner.Advance();

        StringBuilder valueBuilder = new();

        while (!_scanner.IsEnd())
        {
            char c = _scanner.Peek();

            if (c == '\'')
            {
                _scanner.Advance();
                return new Token(TokenType.StringLiteral, valueBuilder.ToString());
            }

            if (c == '\n')
            {
                while (!_scanner.IsEnd() && _scanner.Peek() != '\'')
                {
                    _scanner.Advance();
                }

                if (!_scanner.IsEnd() && _scanner.Peek() == '\'')
                {
                    _scanner.Advance();
                }

                return new Token(TokenType.Error);
            }

            if (c == '\\')
            {
                if (!DecodeEscapeSequence(valueBuilder))
                {
                    _scanner.Advance();

                    while (!_scanner.IsEnd() && _scanner.Peek() != '\'' && _scanner.Peek() != '\n')
                    {
                        _scanner.Advance();
                    }

                    if (!_scanner.IsEnd() && _scanner.Peek() == '\'')
                    {
                        _scanner.Advance();
                    }
                    return new Token(TokenType.Error);
                }
            }
            else
            {
                valueBuilder.Append(c);
                _scanner.Advance();
            }
        }

        return new Token(TokenType.Error);
    }

    /// <summary>
    /// Декодирует escape-последовательность после обратного слэша.
    /// Возвращает true, если последовательность валидна (\\ или \').
    /// </summary>
    private bool DecodeEscapeSequence(StringBuilder valueBuilder)
    {
        _scanner.Advance();

        if (_scanner.IsEnd())
        {
            return false;
        }

        char next = _scanner.Peek();

        if (SimpleEscapes.TryGetValue(next, out char escaped))
        {
            _scanner.Advance();
            valueBuilder.Append(escaped);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Распознаёт идентификаторы и проверяет на ключевые слова.
    /// identifier = (letter | '_'), { letter | digit | '_' }
    /// </summary>
    private Token ParseIdentifierOrKeyword()
    {
        StringBuilder sb = new();
        char c = _scanner.Peek();
        sb.Append(c);
        _scanner.Advance();

        while (!_scanner.IsEnd())
        {
            c = _scanner.Peek();
            if (char.IsAsciiLetter(c) || char.IsAsciiDigit(c) || c == '_')
            {
                sb.Append(c);
                _scanner.Advance();
            }
            else
            {
                break;
            }
        }

        string text = sb.ToString();

        if (Keywords.TryGetValue(text, out TokenType keywordType))
        {
            if (keywordType == TokenType.Identifier)
            {
                return new Token(TokenType.Identifier, text);
            }

            if (text == "true" || text == "false")
            {
                return new Token(text == "true" ? TokenType.True : TokenType.False);
            }

            return new Token(keywordType);
        }

        return new Token(TokenType.Identifier, text);
    }

    /// <summary>
    /// Пропускает пробелы и комментарии (# и /* */).
    /// </summary>
    private void SkipWhiteSpacesAndComments()
    {
        while (true)
        {
            SkipWhiteSpaces();

            if (_scanner.IsEnd())
            {
                break;
            }

            if (SkipHashComment() || SkipMultiLineComment())
            {
                continue;
            }

            break;
        }
    }

    /// <summary>
    /// Пропускает пробельные символы.
    /// </summary>
    private void SkipWhiteSpaces()
    {
        while (!_scanner.IsEnd() && char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    /// <summary>
    /// Пропускает многострочный комментарий /* ... */.
    /// Поддерживает вложенные комментарии через счётчик вложенности.
    /// </summary>
    private bool SkipMultiLineComment()
    {
        if (_scanner.Peek() == '/' && _scanner.Peek(1) == '*')
        {
            _scanner.Advance();
            _scanner.Advance();

            while (!_scanner.IsEnd())
            {
                if (_scanner.Peek() == '*' && _scanner.Peek(1) == '/')
                {
                    break;
                }

                if (!SkipMultiLineComment())
                {
                    _scanner.Advance();
                }
            }

            if (!_scanner.IsEnd() && _scanner.Peek() == '*')
            {
                _scanner.Advance();
            }

            if (!_scanner.IsEnd() && _scanner.Peek() == '/')
            {
                _scanner.Advance();
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Пропускает однострочный комментарий # ... до конца строки.
    /// </summary>
    private bool SkipHashComment()
    {
        if (_scanner.Peek() == '#')
        {
            _scanner.Advance();
            while (!_scanner.IsEnd() && _scanner.Peek() != '\n')
            {
                _scanner.Advance();
            }

            return true;
        }

        return false;
    }
}