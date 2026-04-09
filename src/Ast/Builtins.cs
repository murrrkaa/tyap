using PsTiger.Ast.Declarations;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Ast;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public static class Builtins
{
    public const string ReadInt = "readInt";
    public const string ReadFloat = "readFloat";
    public const string ReadString = "readString";
    public const string Len = "len";
    public const string Substring = "substring";
    public const string ToString = "toString";
    public const string ParseInt = "parseInt";
    public const string ToBool = "toBool";
    public const string ToFloat = "toFloat";

    /// <summary>
    /// Список встроенных функций языка.
    /// </summary>
    public static readonly IReadOnlyList<BuiltinFunction> Functions =
    [
        new(
            ReadInt, // `readInt(): int` — Считывает число, возвращает значение типа int 
            [],
            ValueType.Int
        ),

        new(
            ReadFloat, // `readFloat(): float` — Считывает число, возвращает значение c плавающей точкой типа float
            [],
            ValueType.Float
        ),

        new(
            ReadString, // `readString(): string` — Считывает строку, возвращает значение типа string
            [],
            ValueType.String
        ),

        new(
            Len, // `len(s: string)` - Вычисляет длину строки
            [
                new BuiltinFunctionParameter("s", ValueType.String)
            ], 
            ValueType.Int
        ),

        new(
            Substring, // `substring(s: string, start: int, count: int)` — Получает подстроку, где s - строка, start - начальный индекс, count - кол-во символов строки
            [
                new BuiltinFunctionParameter("s", ValueType.String),
                new BuiltinFunctionParameter("start", ValueType.Int),
                new BuiltinFunctionParameter("count", ValueType.Int),
            ],
            ValueType.String
        ),

        new(
            ToString, // `toString(num: int)` — Преобразует число в строку
            [
                new BuiltinFunctionParameter("i", ValueType.Int),
            ],
            ValueType.String
        ),

        new(
            ParseInt, // `parseInt(str: string)` — Преобразует строку в число
            [
                new BuiltinFunctionParameter("s", ValueType.String),
            ],
            ValueType.Int
        ),

        new(
            ToBool, // `toBool(num: int)` — Преобразует число в булевый тип
            [
                new BuiltinFunctionParameter("i", ValueType.Int),
            ],
            ValueType.Bool
        ),

        new(
            ToFloat, // `toFloat(num: int)` — Преобразует число с плавающей точкой в целое число
            [
                new BuiltinFunctionParameter("i", ValueType.Int),
            ],
            ValueType.Float
        ),
    ];
}