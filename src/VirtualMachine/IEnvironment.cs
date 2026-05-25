namespace Mlt.VirtualMachine;

public interface IEnvironment
{
    /// <summary>
    /// Читает символ из потока ввода либо возвращает -1, если достигнут конец потока.
    /// </summary>
    int ReadChar();

    /// <summary>
    /// Печатает текст в поток вывода.
    /// </summary>
    void Print(string text);

    /// <summary>
    /// Сбрасывает накопленный буфер вывода в поток вывода.
    /// </summary>
    void Flush();
}