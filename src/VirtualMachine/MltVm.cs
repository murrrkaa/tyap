using System;
using System.Collections.Generic;

using Mlt.Runtime;
using Mlt.VirtualMachine.Builtins;
using Mlt.VirtualMachine.Instructions;

namespace Mlt.VirtualMachine;

public class MltVm
{
    private readonly BuiltinFunctions _builtinFunctions;
    private readonly IReadOnlyList<Instruction> _instructions;
    private int _instructionPointer;
    private int _exitCode;
    private readonly Stack<Value> _evaluationStack;

    private readonly Dictionary<string, Value> _variables = new();

    private Value? _result;

    public MltVm(IEnvironment environment, IReadOnlyList<Instruction> instructions)
    {
        ValidateInstructions(instructions);

        _builtinFunctions = new BuiltinFunctions(environment);
        _instructions = instructions;
        _instructionPointer = 0;
        _exitCode = 0;
        _evaluationStack = new Stack<Value>();
        _result = null;
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
                    _evaluationStack.Push(instruction.Operand!);
                    break;

                case InstructionCode.Pop:
                    _evaluationStack.Pop();
                    break;

                case InstructionCode.DefineVar:
                    _variables[instruction.Operand!.AsString()] = _evaluationStack.Pop();
                    break;

                case InstructionCode.StoreVar:
                    _variables[instruction.Operand!.AsString()] = _evaluationStack.Pop();
                    break;

                case InstructionCode.LoadVar:
                    _evaluationStack.Push(_variables[instruction.Operand!.AsString()]);
                    break;

                case InstructionCode.Add:
                    PerformBinaryOp((a, b) => a + b);
                    break;

                case InstructionCode.Subtract:
                    PerformBinaryOp((a, b) => a - b);
                    break;

                case InstructionCode.Multiply:
                    PerformBinaryOp((a, b) => a * b);
                    break;

                case InstructionCode.Divide:
                    PerformBinaryOp((a, b) => a / b);
                    break;

                case InstructionCode.Negate:
                    decimal val = _evaluationStack.Pop().AsDecimal();
                    _evaluationStack.Push(new Value(-val));
                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin(instruction.Operand!);
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

                    return _result ?? new Value(0);

                default:
                    throw new NotImplementedException($"Instruction {instruction.Code} is not supported");
            }
        }
    }

    private void PerformBinaryOp(Func<decimal, decimal, decimal> op)
    {
        decimal b = _evaluationStack.Pop().AsDecimal();
        decimal a = _evaluationStack.Pop().AsDecimal();
        _evaluationStack.Push(new Value(op(a, b)));
    }

    private void CallBuiltin(Value operand)
    {
        string funcName = operand.IsString()
            ? operand.AsString()
            : ((BuiltinFunctionCode)operand.AsInt()).ToString();

        if (string.Equals(funcName, "print", StringComparison.OrdinalIgnoreCase) || funcName == "0")
        {
            _builtinFunctions.Print(_evaluationStack.Pop());
        }
        else
        {
            throw new NotImplementedException($"Builtin {funcName} is not supported");
        }
    }

    private static void ValidateInstructions(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0)
        {
            throw new InvalidOperationException("Empty program");
        }

        if (instructions[instructions.Count - 1].Code != InstructionCode.Halt)
        {
            throw new InvalidOperationException("Program must end with Halt");
        }
    }
}