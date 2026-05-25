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

    [Fact]
    public void Len_Returns_String_Length()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value("hello")),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Len),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(5, vm.ExitCode);
    }

    [Fact]
    public void Substring_Returns_Correct_Value()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value("abcdef")),
            new Instruction(InstructionCode.Push, new Value(1)),
            new Instruction(InstructionCode.Push, new Value(3)),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Substring),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
            new Instruction(InstructionCode.Push, new Value(0)),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal("bcd", environment.BufferedOutput);
    }

    [Fact]
    public void ParseInt_Parses_Number()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value("123")),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ParseInt),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(123, vm.ExitCode);
    }

    [Fact]
    public void ParseInt_Invalid_Throws_Exception()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value("abc")),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ParseInt),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        Assert.Throws<InvalidOperationException>(() => vm.RunProgram());
    }

    [Fact]
    public void ToFloat_Converts_Int_To_Double()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(10)),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ToFloat),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(10, vm.ExitCode);
    }

    [Fact]
    public void ToBool_Converts_NonZero_To_True()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(5)),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ToBool),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
            new Instruction(InstructionCode.Push, new Value(0)),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal("true", environment.BufferedOutput);
    }

    [Fact]
    public void ReadString_Reads_From_Input()
    {
        FakeEnvironment environment = new();
        environment.AddInput("hello\n");

        List<Instruction> program =
        [
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.ReadString),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
            new Instruction(InstructionCode.Push, new Value(0)),
            new Instruction(InstructionCode.Halt),
        ];

        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal("hello", environment.BufferedOutput);
    }
}