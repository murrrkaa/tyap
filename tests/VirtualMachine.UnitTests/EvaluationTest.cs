using Mlt.Runtime;

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
}