using System;
using System.IO;

using PsTiger.Interpreter;

namespace PsTiger.Interpreter;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: PsTiger.Interpreter <file-path>");
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
            TigerInterpreter interpreter = new(environment);

            return interpreter.Execute(sourceCode);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Interpreter error: {ex.Message}");
            return 1;
        }
    }
}