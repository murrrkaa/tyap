using System.Globalization;
using System.Text;

using Mlt.Runtime;

namespace Mlt.VirtualMachine.Builtins;

public class BuiltinFunctions
{
    private readonly IEnvironment _environment;

    public BuiltinFunctions(IEnvironment environment)
    {
        _environment = environment;
    }

    public void Print(Value value)
    {
        _environment.Print(value.ToString());
    }

    public long ReadInt()
    {
        string input = ReadLine();
        if (long.TryParse(input, out long result))
        {
            return result;
        }

        throw new InvalidOperationException($"Невозможно преобразовать '{input}' в int");
    }

    public double ReadFloat()
    {
        string input = ReadLine();
        if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
        {
            return result;
        }

        throw new InvalidOperationException($"Невозможно преобразовать '{input}' в float");
    }

    public string ReadString()
    {
        return ReadLine();
    }

    public long Len(Value text)
    {
        return (long)text.AsString().Length;
    }

    public string Substring(Value value, Value startIndex, Value count)
    {
        string text = value.AsString();
        int start = (int)startIndex.AsLong();
        int length = (int)count.AsLong();

        if (start < 0)
        {
            start = 0;
        }

        if (start >= text.Length)
        {
            return string.Empty;
        }

        if (length < 0)
        {
            length = 0;
        }

        if (start + length > text.Length)
        {
            length = text.Length - start;
        }

        return text.Substring(start, length);
    }

    public long ParseInt(Value value)
    {
        string text = value.AsString();
        if (long.TryParse(text, out long result))
        {
            return result;
        }

        throw new InvalidOperationException($"Невозможно преобразовать '{text}' в int");
    }

    public bool ToBool(Value value)
    {
        return value.AsLong() != 0;
    }

    public double ToFloat(Value value)
    {
        return (double)value.AsLong();
    }

    private string ReadLine()
    {
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            int ch = _environment.ReadChar();
            if (ch == -1 || ch == '\n')
            {
                break;
            }

            if (ch != '\r')
            {
                sb.Append((char)ch);
            }
        }

        return sb.ToString();
    }
}