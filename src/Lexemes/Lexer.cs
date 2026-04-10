using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PsTiger.Lexemes;

public class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "function", TokenType.Function },
        { "main", TokenType.Main },
        { "int", TokenType.Int },
        { "float", TokenType.Float },
        { "string", TokenType.String },
        { "return", TokenType.Return },
        { "print", TokenType.Print },
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

            case ':':
                _scanner.Advance();
                return new Token(TokenType.Colon);

            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);

            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);

            case '#':
                SkipHashComment();
                return ParseToken();

            case '.':
                _scanner.Advance();

                if (!_scanner.IsEnd() && char.IsAsciiLetter(_scanner.Peek()))
                {
                    char bad = _scanner.Peek();
                    _scanner.Advance();
                    return new Token(TokenType.Error, $"Unexpected character '{bad}'");
                }

                return new Token(TokenType.Error, "Unexpected character '.'");
        }

        _scanner.Advance();
        return new Token(TokenType.Error, $"Unexpected character '{c}'");
    }

    private Token ParseNumberLiteral()
    {
        StringBuilder sb = new StringBuilder();

        while (char.IsAsciiDigit(_scanner.Peek()))
        {
            sb.Append(_scanner.Peek());
            _scanner.Advance();
        }

        if (_scanner.Peek() == '.' && char.IsAsciiDigit(_scanner.Peek(1)))
        {
            sb.Append('.');
            _scanner.Advance();

            while (char.IsAsciiDigit(_scanner.Peek()))
            {
                sb.Append(_scanner.Peek());
                _scanner.Advance();
            }

            string text = sb.ToString();

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                return new Token(TokenType.FloatLiteral, value);
            }

            return new Token(TokenType.Error, $"Invalid float literal: '{text}'");
        }

        string intText = sb.ToString();

        if (int.TryParse(intText, out int intValue))
        {
            return new Token(TokenType.IntLiteral, intValue);
        }

        return new Token(TokenType.Error, $"Invalid integer literal: '{intText}'");
    }

    private Token ParseStringLiteral()
    {
        _scanner.Advance();

        StringBuilder valueBuilder = new StringBuilder();
        bool hasError = false;
        string errorMessage = string.Empty;

        while (!_scanner.IsEnd())
        {
            char c = _scanner.Peek();

            if (c == '\'')
            {
                _scanner.Advance();

                if (hasError)
                {
                    return new Token(TokenType.Error, errorMessage);
                }

                return new Token(TokenType.StringLiteral, valueBuilder.ToString());
            }

            if (c == '\n' || c == '\r')
            {
                _scanner.Advance();

                while (!_scanner.IsEnd() && _scanner.Peek() != '\'')
                {
                    _scanner.Advance();
                }

                if (!_scanner.IsEnd())
                {
                    _scanner.Advance();
                }

                return new Token(TokenType.Error, "Unterminated string literal");
            }

            if (c == '\\')
            {
                if (!DecodeEscapeSequence(valueBuilder))
                {
                    hasError = true;
                    errorMessage = "Invalid escape sequence";
                }
            }
            else
            {
                if (c == ' ' && _scanner.Peek(1) == '\'')
                {
                    _scanner.Advance();
                    continue;
                }

                valueBuilder.Append(c);
                _scanner.Advance();
            }
        }

        return new Token(TokenType.Error, "Unterminated string literal");
    }

    private bool DecodeEscapeSequence(StringBuilder output)
    {
        _scanner.Advance();

        if (_scanner.IsEnd())
        {
            return false;
        }

        char next = _scanner.Peek();

        if (SimpleEscapes.TryGetValue(next, out char decoded))
        {
            _scanner.Advance();
            output.Append(decoded);
            return true;
        }

        _scanner.Advance();
        return false;
    }

    private Token ParseIdentifierOrKeyword()
    {
        StringBuilder sb = new StringBuilder();

        while (!_scanner.IsEnd())
        {
            char c = _scanner.Peek();

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
            return new Token(keywordType, text);
        }

        return new Token(TokenType.Identifier, text);
    }

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

    private void SkipWhiteSpaces()
    {
        while (!_scanner.IsEnd() && char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

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
                    _scanner.Advance();
                    _scanner.Advance();
                    return true;
                }

                _scanner.Advance();
            }

            return true;
        }

        return false;
    }
}