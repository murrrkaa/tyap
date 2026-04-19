namespace Mlt.Lexemes;

/// <summary>
/// Типы лексем языка Mlt.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Ключевое слово function
    /// </summary>
    Function,

    /// <summary>
    /// Имя главной функции main
    /// </summary>
    Main,

    /// <summary>
    /// Тип целого числа: int
    /// </summary>
    Int,

    /// <summary>
    /// Тип числа с плавающей точкой: float
    /// </summary>
    Float,

    /// <summary>
    /// Строковый тип: string
    /// </summary>
    String,

    /// <summary>
    /// Ключевое слово return
    /// </summary>
    Return,

    /// <summary>
    /// Встроенная функция вывода: print.
    /// </summary>
    Print,

    /// <summary>
    /// Идентификатор.
    /// </summary>
    Identifier,

    /// <summary>
    /// Литерал целого числа.
    /// </summary>
    IntLiteral,

    /// <summary>
    /// Литерал числа с плавающей точкой.
    /// </summary>
    FloatLiteral,

    /// <summary>
    /// Строковый литерал.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Открывающая фигурная скобка {
    /// </summary>
    OpenBrace,

    /// <summary>
    /// Закрывающая фигурная скобка }
    /// </summary>
    CloseBrace,

    /// <summary>
    /// Открывающая круглая скобка (
    /// </summary>
    OpenParenthesis,

    /// <summary>
    /// Закрывающая круглая скобка )
    /// </summary>
    CloseParenthesis,

    /// <summary>
    /// Двоеточие :
    /// </summary>
    Colon,

    /// <summary>
    /// Запятая ,
    /// </summary>
    Comma,

    /// <summary>
    /// Точка с запятой ;
    /// </summary>
    Semicolon,

    /// <summary>
    /// Конец входного потока
    /// </summary>
    EndOfFile,

    /// <summary>
    /// Ошибка лексического анализа
    /// </summary>
    Error,
}