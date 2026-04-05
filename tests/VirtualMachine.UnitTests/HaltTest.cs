using PsTiger.Tests.TestLibrary.TestDoubles;
using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Instructions;

namespace PsTiger.VirtualMachine.UnitTests;

public class HaltTest
{
    [Theory]
    [MemberData(nameof(GetHaltVmData))]
    public void Can_halt_VM(int exitCode)
    {
        FakeEnvironment environment = new();
        TigerVm vm = new(environment,
        [
            new Instruction(InstructionCode.Push, exitCode),
            new Instruction(InstructionCode.Halt),
        ]);

        vm.RunProgram();

        Assert.Equal(exitCode, vm.ExitCode);
        Assert.Empty(environment.BufferedOutput);
        Assert.Empty(environment.FlushedOutput);
    }

    public static TheoryData<int> GetHaltVmData()
    {
        return
        [
            0,
            1,
        ];
    }
}