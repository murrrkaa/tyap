namespace PsTiger.Lexemes;

/// <summary>
/// Типы лексем языка PsTiger.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Ключевое слово if
    /// </summary>
    If,

    /// <summary>
    /// Ключевое слово else
    /// </summary>
    Else,

    /// <summary>
    /// Ключевое слово for
    /// </summary>
    For,

    /// <summary>
    /// Ключевое слово while
    /// </summary>
    While,

    /// <summary>
    /// Ключевое слово function
    /// </summary>
    Function,

    /// <summary>
    /// Ключевое слово return
    /// </summary>
    Return,

    /// <summary>
    /// Ключевое слово  break
    /// </summary>
    Break,

    /// <summary>
    /// Ключевое слово  continue
    /// </summary>
    Continue,

    /// <summary>
    /// Ключевое слово  var
    /// </summary>
    Var,

    /// <summary>
    /// Ключевое слово  const
    /// </summary>
    Const,

    /// <summary>
    /// Ключевое слово  and
    /// </summary>
    And,

    /// <summary>
    /// Ключевое слово  or
    /// </summary>
    Or,

    /// <summary>
    /// Ключевое слово  int
    /// </summary>
    Int,

    /// <summary>
    /// Ключевое слово float
    /// </summary>
    Float,

    /// <summary>
    /// Ключевое слово string
    /// </summary>
    String,

    /// <summary>
    /// Ключевое слово void
    /// </summary>
    Void,

    /// <summary>
    /// Ключевое слово bool
    /// </summary>
    Bool,

    /// <summary>
    /// Ключевое слово print
    /// </summary>
    Print,

    /// <summary
    /// Логический литерал true
    /// </summary>
    True,

    /// <summary>
    /// Логический литерал false
    /// </summary>
    False,

    /// <summary>
    /// Идентификатор
    /// </summary>
    Identifier,

    /// <summary>
    /// Литерал целого числа
    /// </summary>
    IntLiteral,

    /// <summary>
    /// Литерал числа с плавающей точкой
    /// </summary>
    FloatLiteral,

    /// <summary>
    /// Строковый литерал
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Оператор сложения +
    /// </summary>
    Plus,

    /// <summary>
    /// Оператор вычитания -
    /// </summary>
    Minus,

    /// <summary>
    /// Оператор умножения *
    /// </summary>
    Multiply,

    /// <summary>
    /// Оператор деления /
    /// </summary>
    Divide,

    /// <summary>
    /// Оператор сравнения «равно» ==
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор сравнения «не равно» !=
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор сравнения «меньше»
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения «меньше или равно»
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения «больше»
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения «больше или равно»
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Логический оператор «НЕ»
    /// </summary>
    Not,

    /// <summary>
    /// Оператор присваивания
    /// </summary>
    Assign,

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
    /// Конец входного потока токенов.
    /// </summary>
    EndOfFile,

    /// <summary>
    /// Ошибка лексического анализа.
    /// </summary>
    Error,
}