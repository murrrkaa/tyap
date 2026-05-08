namespace Mlt.Ast.Expressions;

/// <summary>
/// Типы бинарных операций.
/// </summary>
public enum BinaryOperation
{
    /// <summary>
    /// Сложение чисел или конкатенация строк
    /// </summary>
    Add,

    /// <summary>
    /// Вычитание чисел
    /// </summary>
    Subtract,

    /// <summary>
    /// Умножение чисел
    /// </summary>
    Multiply,

    /// <summary>
    /// Деление чисел (целочисленное для int)
    /// </summary>
    Divide,
}