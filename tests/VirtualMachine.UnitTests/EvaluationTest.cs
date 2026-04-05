using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Instructions;

using Xunit;

public class EvaluationTest
{
    [Fact]
    public void Add_Works()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, 2),
            new Instruction(InstructionCode.Push, 3),
            new Instruction(InstructionCode.Add),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        var result = vm.RunProgram();

        Assert.Equal(5, result.AsInt());
    }

    [Fact]
    public void Multiply_Works()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, 4),
            new Instruction(InstructionCode.Push, 5),
            new Instruction(InstructionCode.Multiply),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        var result = vm.RunProgram();

        Assert.Equal(20, result.AsInt());
    }

    [Fact]
    public void Less_Works()
    {
        var env = new FakeEnvironment();
        var program = new[]
        {
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.Push, 2),
            new Instruction(InstructionCode.Less),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt)
        };

        var vm = new TigerVm(env, program);
        var result = vm.RunProgram();

        Assert.Equal(1, result.AsInt());
    }
}