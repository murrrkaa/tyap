namespace PsTiger.VirtualMachine;

public interface IEnvironment
{
    void Print(string text);

    void Flush();
}