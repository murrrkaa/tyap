using PsTiger.Runtime;
using PsTiger.VirtualMachine.Builtins;
using PsTiger.VirtualMachine.Instructions;

using System;
using System.Collections.Generic;

namespace PsTiger.VirtualMachine;

public class TigerVm
{
    private readonly BuiltinFunctions _builtinFunctions;
    private readonly IReadOnlyList<Instruction> _instructions;
    private int _instructionPointer;
    private int _exitCode;
    private readonly Stack<Value> _evaluationStack;
    private Value _result;

    public TigerVm(IEnvironment environment, IReadOnlyList<Instruction> instructions)
    {
        ValidateInstructions(instructions);

        _builtinFunctions = new BuiltinFunctions(environment);
        _instructions = instructions;
        _instructionPointer = 0;
        _exitCode = 0;
        _evaluationStack = new Stack<Value>();
        _result = default(Value);
    }

    public int ExitCode => _exitCode;

    public Value RunProgram()
    {
        while (true)
        {
            Instruction instruction = _instructions[_instructionPointer++];

            switch (instruction.Code)
            {
                case InstructionCode.Push:
                    _evaluationStack.Push(instruction.Operand);
                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin(instruction.Operand);
                    break;

                case InstructionCode.Halt:
                    if (_evaluationStack.Count > 0)
                    {
                        Value finalVal = _evaluationStack.Pop();

                        if (finalVal.IsInt())
                        {
                            _exitCode = finalVal.AsInt();
                        }
                    }

                    return _result;

                default:
                    throw new NotImplementedException(
                        $"Instruction {instruction.Code} is not supported in Epic 1 (1–3)"
                    );
            }
        }
    }

    private void CallBuiltin(Value operand)
    {
        BuiltinFunctionCode code = (BuiltinFunctionCode)operand.AsInt();

        switch (code)
        {
            case BuiltinFunctionCode.Print:
                _builtinFunctions.Print(_evaluationStack.Pop());
                break;

            default:
                throw new NotImplementedException($"Builtin {code} is not allowed in Epic 1");
        }
    }

    private static void ValidateInstructions(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0)
        {
            throw new InvalidOperationException("Empty program is not allowed");
        }

        if (instructions[instructions.Count - 1].Code != InstructionCode.Halt)
        {
            throw new InvalidOperationException("Program must end with Halt");
        }
    }
}