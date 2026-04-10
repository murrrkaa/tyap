using System.Collections.Generic;
using PsTiger.Lexemes;

namespace PsTiger.Parsing;

public class TokenStream
{
    private readonly Lexer _lexer;
    private Token _nextToken;
    private readonly List<Token> _lookupBuffer;

    public TokenStream(string expression)
    {
        _lexer = new Lexer(expression);
        _nextToken = _lexer.ParseToken();
        _lookupBuffer = [];
    }

    public Token Peek()
    {
        return _nextToken;
    }

    public Token Peek(int n)
    {
        if (n == 0)
        {
            return _nextToken;
        }

        while (n > _lookupBuffer.Count)
        {
            Token token = _lexer.ParseToken();
            _lookupBuffer.Add(token);
        }

        return _lookupBuffer[n - 1];
    }

    public void Advance()
    {
        if (_lookupBuffer.Count > 0)
        {
            _nextToken = _lookupBuffer[0];
            _lookupBuffer.RemoveAt(0);
        }
        else
        {
            _nextToken = _lexer.ParseToken();
        }
    }
}