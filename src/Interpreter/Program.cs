using System;
using System.IO;

using Mlt.Interpreter;

namespace Mlt.Interpreter;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: Mlt.Interpreter <file-path>");
            return 1;
        }

        string sourcePath = args[0];
        if (!File.Exists(sourcePath))
        {
            Console.Error.WriteLine($"Error: source file '{sourcePath}' not found.");
            return 1;
        }

        try
        {
            string sourceCode = File.ReadAllText(sourcePath);

            ConsoleEnvironment environment = new();
            MltInterpreter interpreter = new(environment);

            return interpreter.Execute(sourceCode);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Interpreter error: {ex.Message}");
            return 1;
        }
    }
}