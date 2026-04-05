using PsTiger.Runtime;
using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Builtins;
using PsTiger.VirtualMachine.Instructions;

using Xunit;

public class CallBuiltinTest
{
    [Fact]
    public void Print_Writes_String()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, "hello"),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        vm.RunProgram();

        Assert.Equal("hello", env.Output);
    }

    [Fact]
    public void Size_Returns_Length()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, "abc"),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Size),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        var result = vm.RunProgram();

        Assert.Equal(3, result.AsInt());
    }

    [Fact]
    public void Concat_Joins_Strings()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, "ab"),
            new Instruction(InstructionCode.Push, "cd"),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Concat),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        var result = vm.RunProgram();

        Assert.Equal("abcd", result.AsString());
    }
}