using PsTiger.Runtime;
using PsTiger.VirtualMachine.Exceptions;

namespace PsTiger.VirtualMachine.Builtins;

public class BuiltinFunctions
{
    private readonly IEnvironment _environment;

    public BuiltinFunctions(IEnvironment environment)
    {
        _environment = environment;
    }

    public void Print(Value value)
    {
        if (value.IsString())
        {
            _environment.Print(value.AsString());
        }
        else if (value.IsInt())
        {
            _environment.PrintInt(value.AsInt());
        }
        else if (value.IsFloat())
        {
            _environment.Print(value.AsFloat().ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        else if (value.IsBool())
        {
            _environment.Print(value.AsBool() ? "true" : "false");
        }
    }

    public Value ReadInt()
    {
        string input = Console.ReadLine() ?? "0";
        if (int.TryParse(input, out int result))
        {
            return new Value(result);
        }
        return new Value(0);
    }

    public Value ReadFloat()
    {
        string input = Console.ReadLine() ?? "0";
        if (double.TryParse(input, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out double result))
        {
            return new Value(result);
        }
        return new Value(0.0);
    }

    public Value ReadString()
    {
        string input = Console.ReadLine() ?? "";
        return new Value(input);
    }

    public Value Len(Value text)
    {
        return new Value(text.AsString().Length);
    }

    public Value Substring(Value value, Value startIndex, Value count)
    {
        string text = value.AsString();
        int start = startIndex.AsInt();
        int length = count.AsInt();

        if (start < 0) start = 0;
        if (start >= text.Length) return new Value("");
        if (length < 0) length = 0;
        if (start + length > text.Length)
        {
            length = text.Length - start;
        }

        return new Value(text.Substring(start, length));
    }

    public Value ToString(Value value)
    {
        if (value.IsInt())
        {
            return new Value(value.AsInt().ToString());
        }
        else if (value.IsFloat())
        {
            return new Value(value.AsFloat().ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        else if (value.IsBool())
        {
            return new Value(value.AsBool() ? "true" : "false");
        }
        return new Value("");
    }

    public Value ParseInt(Value value)
    {
        string text = value.AsString();
        if (int.TryParse(text, out int result))
        {
            return new Value(result);
        }
        return new Value(0);
    }

    public Value ToBool(Value value)
    {
        if (value.IsInt())
        {
            return new Value(value.AsInt() != 0 ? 1 : 0);
        }
        else if (value.IsFloat())
        {
            return new Value(value.AsFloat() != 0.0 ? 1 : 0);
        }
        return new Value(0);
    }

    public Value ToFloat(Value value)
    {
        if (value.IsInt())
        {
            return new Value((double)value.AsInt());
        }
        return new Value(value.AsFloat());
    }
}