using System;
using System.Globalization;

using PsTiger.VirtualMachine;

namespace PsTiger.Interpreter;

public class ConsoleEnvironment : IEnvironment
{
    public void Print(string text)
    {
        Console.Write(text);
    }

    public void Flush()
    {
        Console.Out.Flush();
    }
}