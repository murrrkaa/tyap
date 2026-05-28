using Mlt.Tests.TestLibrary.TestDoubles;

using Xunit;

namespace TestLibrary.UnitTests;

public class FakeEnvironmentTest
{
    [Fact]
    public void AddInput_and_ReadChar_returns_characters_in_order()
    {
        FakeEnvironment environment = new FakeEnvironment();

        environment.AddInput("abc");

        Assert.Equal('a', environment.ReadChar());
        Assert.Equal('b', environment.ReadChar());
        Assert.Equal('c', environment.ReadChar());
    }

    [Fact]
    public void ReadChar_returns_minus_one_when_input_is_empty()
    {
        FakeEnvironment environment = new FakeEnvironment();

        int result = environment.ReadChar();

        Assert.Equal(-1, result);
    }

    [Fact]
    public void ReadLine_reads_until_newline()
    {
        FakeEnvironment environment = new FakeEnvironment();

        environment.AddInput("hello\nworld");

        string result = environment.ReadLine();

        Assert.Equal("hello", result);
    }

    [Fact]
    public void ReadLine_reads_until_windows_newline()
    {
        FakeEnvironment environment = new FakeEnvironment();

        environment.AddInput("hello\r\nworld");

        string result = environment.ReadLine();

        Assert.Equal("hello", result);
    }

    [Fact]
    public void ReadLine_returns_remaining_text_without_newline()
    {
        FakeEnvironment environment = new FakeEnvironment();

        environment.AddInput("hello");

        string result = environment.ReadLine();

        Assert.Equal("hello", result);
    }

    [Fact]
    public void Print_adds_text_to_buffered_output()
    {
        FakeEnvironment environment = new FakeEnvironment();

        environment.Print("hello");

        Assert.Equal("hello", environment.BufferedOutput);
    }

    [Fact]
    public void PrintInt_adds_integer_to_buffered_output()
    {
        FakeEnvironment environment = new FakeEnvironment();

        environment.PrintInt(123);

        Assert.Equal("123", environment.BufferedOutput);
    }

    [Fact]
    public void Flush_moves_buffered_output_to_flushed_output()
    {
        FakeEnvironment environment = new FakeEnvironment();

        environment.Print("hello");

        environment.Flush();

        Assert.Equal("", environment.BufferedOutput);
        Assert.Equal("hello", environment.FlushedOutput);
    }
}