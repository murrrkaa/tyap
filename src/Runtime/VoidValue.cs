namespace Mlt.Runtime;

public record struct VoidValue
{
    public static readonly VoidValue Value = default;

    public override string ToString()
    {
        return "<void>";
    }
}