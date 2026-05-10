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

    public Instruction(InstructionCode code, decimal value)
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
        if (Operand == null)
        {
            return Code.ToString();
        }

        return $"{Code} {Operand}";
    }
}