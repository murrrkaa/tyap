using System.Collections.Generic;
using System.Globalization;
using System.Text;

using PsTiger.VirtualMachine;

namespace PsTiger.Tests.TestLibrary.TestDoubles;

/// <summary>
/// Тестовая реализация среды выполнения.
/// Имитирует ввод/вывод для виртуальной машины.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly Queue<char> _input = new();
    private readonly StringBuilder _bufferedOutput = new();
    private readonly StringBuilder _flushedOutput = new();

    /// <summary>
    /// Вывод, который еще не был сброшен через Flush()
    /// </summary>
    public string BufferedOutput => _bufferedOutput.ToString();

    /// <summary>
    /// Вывод, который был сброшен через Flush()
    /// </summary>
    public string FlushedOutput => _flushedOutput.ToString();

    /// <summary>
    /// Добавить входные данные для getchar
    /// </summary>
    public void AddInput(string text)
    {
        foreach (char c in text)
        {
            _input.Enqueue(c);
        }
    }

    /// <summary>
    /// getchar()
    /// Возвращает следующий символ или -1 если ввод закончился
    /// </summary>
    public int ReadChar()
    {
        if (_input.TryDequeue(out char c))
        {
            return c;
        }

        return -1;
    }

    /// <summary>
    /// print()
    /// </summary>
    public void Print(string text)
    {
        _bufferedOutput.Append(text);
    }

    /// <summary>
    /// printi()
    /// </summary>
    public void PrintInt(int value)
    {
        _bufferedOutput.Append(value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// flush()
    /// </summary>
    public void Flush()
    {
        _flushedOutput.Append(_bufferedOutput.ToString());
        _bufferedOutput.Clear();
    }
}