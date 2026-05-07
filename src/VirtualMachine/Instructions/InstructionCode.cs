namespace Mlt.VirtualMachine.Instructions;

public enum InstructionCode
{
    /// <summary>
    /// Добавляет значение (decimal или string) в стек вычислений.
    /// </summary>
    Push,

    /// <summary>
    /// Удаляет значение с вершины стека вычислений.
    /// </summary>
    Pop,

    /// <summary>
    /// Забирает значение из стека, записывает его в новую переменную.
    /// </summary>
    DefineVar,

    /// <summary>
    /// Забирает значение из стека, записывает его в существующую переменную.
    /// </summary>
    StoreVar,

    /// <summary>
    /// Читает значение переменной и кладет его в стек вычислений.
    /// </summary>
    LoadVar,

    /// <summary>
    /// Складывает два числа на стеке.
    /// </summary>
    Add,

    /// <summary>
    /// Вычитает два числа на стеке.
    /// </summary>
    Subtract,

    /// <summary>
    /// Умножает два числа на стеке.
    /// </summary>
    Multiply,

    /// <summary>
    /// Делит два числа на стеке.
    /// </summary>
    Divide,

    /// <summary>
    /// Меняет знак числа на вершине стека.
    /// </summary>
    Negate,

    /// <summary>
    /// Выполняет вызов встроенной функции (например, print).
    /// </summary>
    CallBuiltin,

    /// <summary>
    /// Останавливает выполнение программы.
    /// </summary>
    Halt,
}