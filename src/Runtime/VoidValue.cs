namespace PsTiger.Runtime;

/// <summary>
/// Специальный тип, обозначающий отсутствие значения (void).
/// </summary>
public record struct VoidValue
{
    /// <summary>
    /// Единственный экземпляр значения void.
    /// </summary>
    public static readonly VoidValue Value = default;

    public override string ToString()
    {
        return "<void>";
    }
}