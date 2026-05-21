namespace Mlt.VirtualMachine;

public interface IEnvironment
{
    void Print(string text);
    void Flush();
    string ReadLine();
}