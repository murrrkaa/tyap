using System.Globalization;

using PsTiger.Runtime;

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
            _environment.Print(value.AsInt().ToString(CultureInfo.InvariantCulture));
        }
        else if (value.IsFloat()) 
        {
            _environment.Print(value.AsFloat().ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            _environment.Print(value.ToString());
        }
    }
}