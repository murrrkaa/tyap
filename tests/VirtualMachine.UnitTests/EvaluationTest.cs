using Mlt.Runtime;
using Mlt.Tests.TestLibrary.TestDoubles;
using Mlt.VirtualMachine.Instructions;

using Xunit;

namespace Mlt.VirtualMachine.UnitTests;

using VmValueType = Mlt.Runtime.ValueType;

public class EvaluationTest
{
    [Fact]
    public void Value_Equals_String()
    {
        Value a = new("abc");
        Value b = new("abc");

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
    }

    [Fact]
    public void Value_Equals_Int()
    {
        Value a = new(42);
        Value b = new(42);

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Value_Equals_Float()
    {
        Value a = new(3.14);
        Value b = new(3.14);

        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Value_NotEquals_Null()
    {
        Value a = new(1);

        Assert.False(a.Equals(null));
    }

    [Fact]
    public void Value_NotEquals_DifferentTypes()
    {
        Value a = new(1);
        Value b = new("1");

        Assert.False(a.Equals(b));
    }

    [Fact]
    public void ValueType_ToString_ReturnsName()
    {
        Assert.Equal("int", VmValueType.Int.ToString());
        Assert.Equal("float", VmValueType.Float.ToString());
        Assert.Equal("string", VmValueType.String.ToString());
    }

    [Theory]
    [InlineData("", "''")]
    [InlineData("abc", "'abc'")]
    [InlineData("a'b", @"'a\'b'")]
    [InlineData(@"a\b", @"'a\\b'")]
    [InlineData(@"a'\b", @"'a\'\\b'")]
    public void EscapeStringValue_Works(string input, string expected)
    {
        string result = ValueUtil.EscapeStringValue(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_Ints_Works()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, 5L),
            new Instruction(InstructionCode.Push, 3L),
            new Instruction(InstructionCode.Add),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(8, vm.ExitCode);
    }

    [Fact]
    public void Divide_By_Zero_Throws()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, 10L),
            new Instruction(InstructionCode.Push, 0L),
            new Instruction(InstructionCode.Divide),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        Assert.Throws<InvalidOperationException>(() => vm.RunProgram());
    }

    [Fact]
    public void DefineVar_And_LoadVar_Work()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, 42L),
            new Instruction(InstructionCode.DefineVar, "x"),
            new Instruction(InstructionCode.LoadVar, "x"),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(42, vm.ExitCode);
    }

    [Fact]
    public void Equal_Returns_True()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, 5L),
            new Instruction(InstructionCode.Push, 5L),
            new Instruction(InstructionCode.Equal),
            new Instruction(InstructionCode.CallBuiltin, 0),
            new Instruction(InstructionCode.Push, 0L),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal("true", environment.BufferedOutput);
    }

    [Fact]
    public void Not_Works()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(true)),
            new Instruction(InstructionCode.Not),
            new Instruction(InstructionCode.CallBuiltin, 0),
            new Instruction(InstructionCode.Push, 0L),
            new Instruction(InstructionCode.Halt),
        ];

        FakeEnvironment environment = new();
        MltVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal("false", environment.BufferedOutput);
    }
}