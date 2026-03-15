namespace PsTiger.Lexemes;

/// <summary>
/// Типы лексем языка PsTiger.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Ключевое слово <c>if</c>.
    /// </summary>
    If,

    /// <summary>
    /// Ключевое слово <c>else</c>.
    /// </summary>
    Else,

    /// <summary>
    /// Ключевое слово <c>for</c>.
    /// </summary>
    For,

    /// <summary>
    /// Ключевое слово <c>while</c>.
    /// </summary>
    While,

    /// <summary>
    /// Ключевое слово <c>function</c>.
    /// </summary>
    Function,

    /// <summary>
    /// Ключевое слово <c>return</c>.
    /// </summary>
    Return,

    /// <summary>
    /// Ключевое слово <c>break</c>.
    /// </summary>
    Break,

    /// <summary>
    /// Ключевое слово <c>continue</c>.
    /// </summary>
    Continue,

    /// <summary>
    /// Ключевое слово <c>var</c>.
    /// </summary>
    Var,

    /// <summary>
    /// Ключевое слово <c>const</c>.
    /// </summary>
    Const,

    /// <summary>
    /// Ключевое слово <c>and</c>.
    /// </summary>
    And,

    /// <summary>
    /// Ключевое слово <c>or</c>.
    /// </summary>
    Or,

    /// <summary>
    /// Ключевое слово <c>int</c> (тип данных).
    /// </summary>
    Int,

    /// <summary>
    /// Ключевое слово <c>float</c> (тип данных).
    /// </summary>
    Float,

    /// <summary>
    /// Ключевое слово <c>string</c> (тип данных).
    /// </summary>
    String,

    /// <summary>
    /// Ключевое слово <c>void</c> (тип данных).
    /// </summary>
    Void,

    /// <summary>
    /// Ключевое слово <c>bool</c> (тип данных).
    /// </summary>
    Bool,

    /// <summary>
    /// Ключевое слово <c>print</c> (встроенная функция вывода).
    /// </summary>
    Print,

    /// <summary>
    /// Логический литерал <c>true</c>.
    /// </summary>
    True,

    /// <summary>
    /// Логический литерал <c>false</c>.
    /// </summary>
    False,

    /// <summary>
    /// Идентификатор (имя переменной, функции и т.д.).
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
    /// Оператор сложения <c>+</c>.
    /// </summary>
    Plus,

    /// <summary>
    /// Оператор вычитания <c>-</c>.
    /// </summary>
    Minus,

    /// <summary>
    /// Оператор умножения <c>*</c>.
    /// </summary>
    Multiply,

    /// <summary>
    /// Оператор деления <c>/</c>.
    /// </summary>
    Divide,

    /// <summary>
    /// Оператор сравнения «равно» <c>==</c>.
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор сравнения «не равно» <c>!=</c>.
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор сравнения «меньше» <c>&lt;</c>.
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения «меньше или равно» <c>&lt;=</c>.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения «больше» <c>&gt;</c>.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения «больше или равно» <c>&gt;=</c>.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Логический оператор «НЕ» <c>!</c>.
    /// </summary>
    Not,

    /// <summary>
    /// Оператор присваивания <c>=</c>.
    /// </summary>
    Assign,

    /// <summary>
    /// Открывающая фигурная скобка <c>{</c>.
    /// </summary>
    OpenBrace,

    /// <summary>
    /// Закрывающая фигурная скобка <c>}</c>.
    /// </summary>
    CloseBrace,

    /// <summary>
    /// Открывающая круглая скобка <c>(</c>.
    /// </summary>
    OpenParenthesis,

    /// <summary>
    /// Закрывающая круглая скобка <c>)</c>.
    /// </summary>
    CloseParenthesis,

    /// <summary>
    /// Запятая <c>,</c>.
    /// </summary>
    Comma,

    /// <summary>
    /// Точка с запятой <c>;</c>.
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