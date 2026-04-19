namespace Mlt.Lexemes;

public class TextScanner
{
    private readonly string _input;
    private int _position;

    public TextScanner(string input)
    {
        _input = input;
        _position = 0;
    }

    public char Peek(int n = 0)
    {
        int position = _position + n;
        return position >= _input.Length ? '\0' : _input[position];
    }

    public void Advance()
    {
        _position++;
    }

    public bool IsEnd()
    {
        return _position >= _input.Length;
    }
}