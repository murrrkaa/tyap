using System.Collections.Generic;

using PsTiger.Tests.TestLibrary.TestDoubles;
using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Builtins;

using PsTiger.VirtualMachine.Instructions;

using Xunit;

namespace PsTiger.VirtualMachine.UnitTests;

public class CallBuiltinTest
{
    [Theory]
    [MemberData(nameof(GetPrintData))]
    public void Print_Writes_To_BufferedOutput(
        List<Instruction> program,
        string expectedBufferedOutput)
    {
        FakeEnvironment environment = new();
        TigerVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expectedBufferedOutput, environment.BufferedOutput);
        Assert.Equal(string.Empty, environment.FlushedOutput);
    }

    public static TheoryData<List<Instruction>, string> GetPrintData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            // print string
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, "Hello, world!"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                },
                "Hello, world!"
            },

            // print number via toString
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, 42),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ToString),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                },
                "42"
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetBuiltinFunctionsData))]
    public void Builtin_Functions_Work(
        List<Instruction> program,
        string expectedBufferedOutput)
    {
        FakeEnvironment environment = new();
        TigerVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expectedBufferedOutput, environment.BufferedOutput);
    }

    public static TheoryData<List<Instruction>, string> GetBuiltinFunctionsData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            // len
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, "Hello"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Len),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ToString),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                },
                "5"
            },

            // substring
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, "Hello, world!"),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Push, 5),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Substring),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                },
                "Hello"
            },

            // parseInt
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, "123"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ParseInt),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ToString),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                },
                "123"
            },

            // toFloat
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, 42),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ToFloat),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ToString),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                },
                "42"
            },
        };
    }
}