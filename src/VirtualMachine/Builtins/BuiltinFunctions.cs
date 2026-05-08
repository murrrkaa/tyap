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
        if (value.IsString())
        {
            _environment.Print(value.AsString());
        }
        else if (value.IsInt() || value.IsFloat())
        {
            _environment.Print(value.AsDecimal().ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            _environment.Print(value.ToString() ?? "");
        }
    }
}