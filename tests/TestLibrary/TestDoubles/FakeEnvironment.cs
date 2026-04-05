using PsTiger.VirtualMachine;

public class FakeEnvironment : IEnvironment
{
    public string Output = "";

    public int ReadChar() => -1;

    public void Print(string text)
    {
        Output += text;
    }

    public void PrintInt(int value)
    {
        Output += value.ToString();
    }

    public void Flush()
    {
    }
}