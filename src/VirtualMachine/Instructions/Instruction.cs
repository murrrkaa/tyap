using System.Text;

using Mlt.Runtime;

namespace Mlt.VirtualMachine.Instructions;

public class Instruction
{
    public Instruction(InstructionCode code)
    {
        Code = code;
        Operand = null;
    }

    public Instruction(InstructionCode code, int value)
    {
        Code = code;
        Operand = new Value(value);
    }

    public Instruction(InstructionCode code, string value)
    {
        Code = code;
        Operand = new Value(value);
    }

    public Instruction(InstructionCode code, Value value)
    {
        Code = code;
        Operand = value;
    }

    public InstructionCode Code { get; }

    public Value? Operand { get; }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(Code);

        return sb.ToString();
    }
}