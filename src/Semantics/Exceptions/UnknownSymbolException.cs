using System;

namespace Mlt.Semantics.Exceptions;

#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
/// <summary>
/// Исключение из-за отсутствия символа с указанным именем в таблице символов.
/// </summary>
public class UnknownSymbolException : Exception
{
    private UnknownSymbolException(string name, string message)
        : base(message)
    {
        Name = name;
    }

    public string Name { get; }

    public static UnknownSymbolException UndefinedVariable(string name)
    {
        return new UnknownSymbolException(
            name,
            $"Variable '{name}' is not defined in the current scope"
        );
    }

    public static UnknownSymbolException UndefinedType(string name)
    {
        return new UnknownSymbolException(
            name,
            $"Type '{name}' is unknown or not supported"
        );
    }
}
#pragma warning restore RCS1194