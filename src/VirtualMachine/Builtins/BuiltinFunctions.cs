using System;
using System.Globalization;

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
        string input = _environment.ReadLine();
        if (long.TryParse(input, out long result))
            return result;
        throw new InvalidOperationException($"Cannot parse '{input}' as int");
    }

    public decimal ReadFloat()
    {
        string input = _environment.ReadLine();
        if (decimal.TryParse(input, NumberStyles.Float,
                CultureInfo.InvariantCulture, out decimal result))
            return result;
        throw new InvalidOperationException($"Cannot parse '{input}' as float");
    }

    public string ReadString()
    {
        return _environment.ReadLine();
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

        if (start < 0) start = 0;
        if (start >= text.Length) return "";
        if (length < 0) length = 0;
        if (start + length > text.Length) length = text.Length - start;

        return text.Substring(start, length);
    }

    public long ParseInt(Value value)
    {
        string text = value.AsString();
        if (long.TryParse(text, out long result))
            return result;
        throw new InvalidOperationException($"Cannot parse '{text}' as int");
    }

    public bool ToBool(Value value)
    {
        return value.AsLong() != 0;
    }

    public decimal ToFloat(Value value)
    {
        return (decimal)value.AsLong();
    }
}