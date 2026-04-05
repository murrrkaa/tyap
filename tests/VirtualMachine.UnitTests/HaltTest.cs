using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Instructions;

using Xunit;

public class HaltTest
{
    [Fact]
    public void Halt_Returns_ExitCode()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, 5),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        vm.RunProgram();

        Assert.Equal(5, vm.ExitCode);
    }

    [Fact]
    public void StoreResult_Returns_Value()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, 42),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        var result = vm.RunProgram();

        Assert.Equal(42, result.AsInt());
    }
}