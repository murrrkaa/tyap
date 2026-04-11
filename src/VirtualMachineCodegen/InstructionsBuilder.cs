using System;
using System.Collections.Generic;

using PsTiger.VirtualMachine.Instructions;

namespace PsTiger.VirtualMachineCodegen;

public class InstructionsBuilder
{
    private readonly List<Instruction> _instructions;

    public InstructionsBuilder()
    {
        _instructions = new List<Instruction>();
    }

    public List<Instruction> Finish()
    {
        return _instructions;
    }

    public void Append(Instruction instruction)
    {
        _instructions.Add(instruction);
    }
}