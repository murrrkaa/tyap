using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Mlt.VirtualMachine;

namespace Mlt.Tests.TestLibrary.TestDoubles;

public class FakeEnvironment : IEnvironment
{
    private readonly Queue<char> _input = new();
    private readonly StringBuilder _bufferedOutput = new();
    private readonly StringBuilder _flushedOutput = new();

    public string BufferedOutput => _bufferedOutput.ToString();

    public string FlushedOutput => _flushedOutput.ToString();

    public void AddInput(string text)
    {
        foreach (char c in text)
        {
            _input.Enqueue(c);
        }
    }

    public int ReadChar()
    {
        if (_input.TryDequeue(out char c))
        {
            return c;
        }

        return -1;
    }

    public string ReadLine()
    {
        StringBuilder sb = new StringBuilder();

        while (_input.TryDequeue(out char c))
        {
            if (c == '\n')
            {
                break;
            }
            if (c == '\r')
            {
                if (_input.TryPeek(out char next) && next == '\n')
                {
                    _input.Dequeue();
                }
                break;
            }
            sb.Append(c);
        }

        return sb.ToString();
    }

    public void Print(string text)
    {
        _bufferedOutput.Append(text);
    }

    public void PrintInt(int value)
    {
        _bufferedOutput.Append(value.ToString(CultureInfo.InvariantCulture));
    }

    public void Flush()
    {
        _flushedOutput.Append(_bufferedOutput.ToString());
        _bufferedOutput.Clear();
    }
}