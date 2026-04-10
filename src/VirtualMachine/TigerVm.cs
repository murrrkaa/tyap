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
    private readonly Stack<ReturnContext> _returnStack;
    private Value _result;

    public TigerVm(IEnvironment environment, IReadOnlyList<Instruction> instructions)
    {
        ValidateInstructions(instructions);

        _builtinFunctions = new BuiltinFunctions(environment);
        _instructions = instructions;
        _instructionPointer = 0;
        _exitCode = 0;
        _evaluationStack = new Stack<Value>();
        _returnStack = new Stack<ReturnContext>();
        _result = Value.Void;
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

                case InstructionCode.Pop:
                    if (_evaluationStack.Count > 0) _evaluationStack.Pop();
                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin((BuiltinFunctionCode)instruction.Operand.AsInt());
                    break;

                case InstructionCode.Return:
                    {
                        Value returnValue = Value.Void;
                        if (_evaluationStack.Count > 0)
                        {
                            returnValue = _evaluationStack.Pop();
                        }

                        if (_returnStack.Count == 0)
                        {
                            _result = returnValue;
                            if (returnValue.IsInt()) _exitCode = returnValue.AsInt();
                            return _result;
                        }
                        else
                        {
                            if (returnValue != Value.Void) _evaluationStack.Push(returnValue);
                            ReturnContext context = _returnStack.Pop();
                            _instructionPointer = context.InstructionPointer;
                        }
                    }
                    break;

                case InstructionCode.Halt:
                    if (_evaluationStack.Count > 0)
                    {
                        var finalVal = _evaluationStack.Pop();
                        if (finalVal.IsInt()) _exitCode = finalVal.AsInt();
                    }
                    return _result;

                default:
                    throw new NotImplementedException($"Instruction {instruction.Code} is disabled for Epic 1 (Points 1-3)");
            }
        }
    }

    private void CallBuiltin(BuiltinFunctionCode code)
    {
        if (code == BuiltinFunctionCode.Print)
        {
            _builtinFunctions.Print(_evaluationStack.Pop());
        }
        else
        {
            throw new ArgumentException($"Builtin function {code} is not implemented in this iteration.");
        }
    }

    private static void ValidateInstructions(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0)
            throw new InvalidOperationException("Invalid empty VM program");

        InstructionCode last = instructions[^1].Code;
        if (last != InstructionCode.Halt && last != InstructionCode.Return)
        {
            throw new InvalidOperationException("Program must end with Halt or Return");
        }
    }

    private record struct ReturnContext(int InstructionPointer);
}