using System.Collections.Generic;

using PsTiger.VirtualMachine.Instructions;

namespace PsTiger.VirtualMachineCodegen;

/// <summary>
/// Базовый блок инструкций — это линейная последовательность инструкций виртуальной машины,
///  которая обычно завершается переходом, возвратом либо остановом.
/// </summary>
/// <remarks>
/// В нашем бэкенде, в отличие от LLVM, нет строго правила "базовый блок имеет ровно одну завершающую инструкцию".
/// </remarks>
public class BasicBlock
{
    private readonly int _id;
    private readonly List<Instruction> _instructions;

    public BasicBlock(int id)
    {
        _id = id;
        _instructions = [];
    }

    public int Id => _id;

    public List<Instruction> Instructions => _instructions;

    public void Append(Instruction instruction)
    {
        _instructions.Add(instruction);
    }
}