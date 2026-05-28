using System;

using Mlt.Runtime;
using Mlt.Tests.TestLibrary.TestDoubles;
using Mlt.VirtualMachine.Builtins;

using Xunit;

namespace VirtualMachine.UnitTests;

public class BuiltinFunctionsTest
{
    [Fact]
    public void Print_outputs_value_to_environment()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        builtin.Print(new Value(123L));

        Assert.Equal("123", environment.BufferedOutput);
    }

    [Fact]
    public void ReadInt_reads_integer()
    {
        FakeEnvironment environment = new();

        environment.AddInput("123\n");

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        long result = builtin.ReadInt();

        Assert.Equal(123, result);
    }

    [Fact]
    public void ReadInt_invalid_number_throws_exception()
    {
        FakeEnvironment environment = new();

        environment.AddInput("abc\n");

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => builtin.ReadInt());

        Assert.Equal("Невозможно преобразовать 'abc' в int", ex.Message);
    }

    [Fact]
    public void ReadFloat_reads_float()
    {
        FakeEnvironment environment = new();

        environment.AddInput("12.5\n");

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        double result = builtin.ReadFloat();

        Assert.Equal(12.5, result);
    }

    [Fact]
    public void ReadFloat_invalid_value_throws_exception()
    {
        FakeEnvironment environment = new();

        environment.AddInput("abc\n");

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => builtin.ReadFloat());

        Assert.Equal("Невозможно преобразовать 'abc' в float", ex.Message);
    }

    [Fact]
    public void ReadString_reads_text()
    {
        FakeEnvironment environment = new();

        environment.AddInput("hello world\n");

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        string result = builtin.ReadString();

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void Len_returns_string_length()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        long result = builtin.Len(new Value("hello"));

        Assert.Equal(5, result);
    }

    [Fact]
    public void Substring_returns_part_of_string()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        string result = builtin.Substring(
            new Value("hello"),
            new Value(1L),
            new Value(3L));

        Assert.Equal("ell", result);
    }

    [Fact]
    public void Substring_negative_start_uses_zero()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        string result = builtin.Substring(
            new Value("hello"),
            new Value(-1L),
            new Value(2L));

        Assert.Equal("he", result);
    }

    [Fact]
    public void Substring_start_out_of_range_returns_empty()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        string result = builtin.Substring(
            new Value("hello"),
            new Value(10L),
            new Value(2L));

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Substring_negative_length_returns_empty()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        string result = builtin.Substring(
            new Value("hello"),
            new Value(1L),
            new Value(-5L));

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Substring_trims_length_to_end_of_string()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        string result = builtin.Substring(
            new Value("hello"),
            new Value(3L),
            new Value(10L));

        Assert.Equal("lo", result);
    }

    [Fact]
    public void ParseInt_converts_string_to_int()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        long result = builtin.ParseInt(new Value("123"));

        Assert.Equal(123, result);
    }

    [Fact]
    public void ParseInt_invalid_value_throws_exception()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => builtin.ParseInt(new Value("abc")));

        Assert.Equal("Невозможно преобразовать 'abc' в int", ex.Message);
    }

    [Fact]
    public void ToBool_zero_returns_false()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        bool result = builtin.ToBool(new Value(0L));

        Assert.False(result);
    }

    [Fact]
    public void ToBool_non_zero_returns_true()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        bool result = builtin.ToBool(new Value(10L));

        Assert.True(result);
    }

    [Fact]
    public void ToFloat_converts_long_to_double()
    {
        FakeEnvironment environment = new();

        BuiltinFunctions builtin = new BuiltinFunctions(environment);

        double result = builtin.ToFloat(new Value(10L));

        Assert.Equal(10.0, result);
    }
}