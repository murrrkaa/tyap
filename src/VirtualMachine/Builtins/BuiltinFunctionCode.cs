namespace Mlt.VirtualMachine.Builtins;

/// <summary>
/// Код встроенной функции.
/// </summary>
public enum BuiltinFunctionCode
{
    /// <summary>
    /// `print(...)` — выводит значения в стандартный поток вывода
    /// </summary>
    Print = 0,

    /// <summary>
    /// `readInt(): int` — считывает число типа int
    /// </summary>
    ReadInt = 1,

    /// <summary>
    /// `readFloat(): float` — считывает число типа float
    /// </summary>
    ReadFloat = 2,

    /// <summary>
    /// `readString(): string` — считывает строку
    /// </summary>
    ReadString = 3,

    /// <summary>
    /// `len(s: string): int` — возвращает длину строки
    /// </summary>
    Len = 4,

    /// <summary>
    /// `substring(s: string, start: int, count: int): string` — возвращает подстроку
    /// </summary>
    Substring = 5,

    /// <summary>
    /// `toString(num): string` — преобразует число в строку
    /// </summary>
    ToString = 6,

    /// <summary>
    /// `parseInt(str: string): int` — преобразует строку в число
    /// </summary>
    ParseInt = 7,

    /// <summary>
    /// `toBool(num): bool` — преобразует число в булевый тип
    /// </summary>
    ToBool = 8,

    /// <summary>
    /// `toFloat(num): float` — преобразует int в float
    /// </summary>
    ToFloat = 9,
}