using System.Collections.Generic;

using Mlt.Runtime;
using Mlt.Tests.TestLibrary.TestDoubles;
using Mlt.VirtualMachine;
using Mlt.VirtualMachine.Builtins;
using Mlt.VirtualMachine.Instructions;

using Xunit;

namespace Mlt.VirtualMachine.UnitTests;

public class CallBuiltinTest
{
    [Theory]
    [MemberData(nameof(GetPrintData))]
    public void Print_Writes_To_BufferedOutput(
        List<Instruction> program,
        string expectedBufferedOutput)
    {
        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expectedBufferedOutput, environment.BufferedOutput);
        Assert.Equal(string.Empty, environment.FlushedOutput);
    }

    public static TheoryData<List<Instruction>, string> GetPrintData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, new Value("Hello")),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, new Value(0)),
                    new Instruction(InstructionCode.Halt),
                },
                "Hello"
            },
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, new Value(42)),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, new Value(0)),
                    new Instruction(InstructionCode.Halt),
                },
                "42"
            },
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, new Value(3.14)),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, new Value(0)),
                    new Instruction(InstructionCode.Halt),
                },
                "3.14"
            },
            {
                new List<Instruction>
                {
                    new Instruction(InstructionCode.Push, new Value(1)),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, new Value("test")),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, new Value(2.5)),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, new Value(0)),
                    new Instruction(InstructionCode.Halt),
                },
                "1test2.5"
            },
        };
    }

    [Fact]
    public void Return_Sets_ExitCode()
    {
        List<Instruction> program = new List<Instruction>
        {
            new Instruction(InstructionCode.Push, new Value(1)),
            new Instruction(InstructionCode.Halt),
        };

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(1, vm.ExitCode);
        Assert.Equal(string.Empty, environment.BufferedOutput);
    }
}