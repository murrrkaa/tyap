using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Instructions;

using Xunit;

public class TigerVmExitTests
{
    [Fact]
    public void Should_Halt_With_Zero_Code()
    {
        var instructions = new[]
        {
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(new TestEnvironment(), instructions);

        vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
    }

    [Fact]
    public void Should_Halt_With_NonZero_Code()
    {
        var instructions = new[]
        {
            new Instruction(InstructionCode.Push, 5),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(new TestEnvironment(), instructions);

        vm.RunProgram();

        Assert.Equal(5, vm.ExitCode);
    }
}
