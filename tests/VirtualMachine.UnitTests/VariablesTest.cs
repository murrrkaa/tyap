using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Instructions;

using Xunit;

public class VariablesTest
{
    [Fact]
    public void Define_And_Load_Variable()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, 10),
            new Instruction(InstructionCode.DefineVar, "x"),
            new Instruction(InstructionCode.LoadVar, "x"),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        var result = vm.RunProgram();

        Assert.Equal(10, result.AsInt());
    }

    [Fact]
    public void Store_Variable()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.DefineVar, "x"),
            new Instruction(InstructionCode.Push, 5),
            new Instruction(InstructionCode.StoreVar, "x"),
            new Instruction(InstructionCode.LoadVar, "x"),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        var result = vm.RunProgram();

        Assert.Equal(5, result.AsInt());
    }
}