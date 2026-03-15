namespace PsTiger.Lexemes;

/// <summary>
/// Сканирует исходный код в виде строки, предоставляя три операции: Peek(N), Advance() и IsEnd().
/// </summary>
public class TextScanner
{
    private readonly string _input;
    private int _position;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TextScanner"/>.
    /// </summary>
    /// <param name="input">Входная строка для сканирования.</param>
    public TextScanner(string input)
    {
        _input = input;
        _position = 0;
    }

    /// <summary>
    /// Читает символ на N позиций вперёд от текущей позиции (по умолчанию N=0).
    /// </summary>
    /// <param name="n">Смещение относительно текущей позиции.</param>
    /// <returns>Символ по указанной позиции или '\0', если позиция за пределами строки.</returns>
    public char Peek(int n = 0)
    {
        int position = _position + n;
        return position >= _input.Length ? '\0' : _input[position];
    }

    /// <summary>
    /// Сдвигает текущую позицию на один символ вперёд.
    /// </summary>
    public void Advance()
    {
        _position++;
    }

    /// <summary>
    /// Проверяет, достигли ли мы конца входных данных.
    /// </summary>
    /// <returns>True, если позиция достигла конца строки; иначе False.</returns>
    public bool IsEnd()
    {
        return _position >= _input.Length;
    }
}