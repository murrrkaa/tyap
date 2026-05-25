using System;

using Mlt.VirtualMachine;

namespace Mlt.Interpreter;

public class ConsoleEnvironment : IEnvironment
{
    public int ReadChar()
    {
        return Console.Read();
    }

    public void Print(string text)
    {
        Console.Write(text);
    }

    public void Flush()
    {
        Console.Out.Flush();
    }
}