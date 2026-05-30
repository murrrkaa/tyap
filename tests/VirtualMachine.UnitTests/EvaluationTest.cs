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
        Assert.Equal("bool", VmValueType.Bool.ToString());
    }

    [Fact]
    public void Value_Type_Checks()
    {
        Assert.True(new Value("a").IsString());
        Assert.True(new Value(10L).IsInt());
        Assert.True(new Value(10.5).IsFloat());
        Assert.True(new Value(true).IsBool());
    }

    [Fact]
    public void Value_Equals_Should_Work()
    {
        Assert.True(new Value(10L).Equals(new Value(10L)));
        Assert.True(new Value("a").Equals(new Value("a")));
        Assert.True(new Value(true).Equals(new Value(true)));
    }

    [Fact]
    public void Value_Invalid_Cast_Should_Throw()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new Value("abc").AsLong());
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

    [Fact]
    public void Value_AsString_Invalid_Type_Should_Throw()
    {
        Value value = new(10L);

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => value.AsString());

        Assert.Equal("Значение 10 не является строкой.", ex.Message);
    }

    [Fact]
    public void Value_AsDouble_From_Long_Should_Work()
    {
        Value value = new(10L);

        double result = value.AsDouble();

        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Value_AsDouble_Invalid_Type_Should_Throw()
    {
        Value value = new("abc");

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => value.AsDouble());

        Assert.Contains("Значение abc не является числом с плавающей запятой.", ex.Message);
    }

    [Fact]
    public void Value_AsBool_Invalid_Type_Should_Throw()
    {
        Value value = new("abc");

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => value.AsBool());

        Assert.Equal("Значение abc не является логическим значением.", ex.Message);
    }

    [Fact]
    public void Value_GetHashCode_Should_Work()
    {
        Value value = new(123L);

        int hash = value.GetHashCode();

        Assert.NotEqual(0, hash);
    }
}