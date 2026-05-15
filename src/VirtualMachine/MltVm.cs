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
    private long _exitCode;
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

    public long ExitCode => _exitCode;

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
                    PerformAdd();
                    break;

                case InstructionCode.Subtract:
                    PerformIntOrFloatOp((a, b) => a - b, (a, b) => a - b);
                    break;

                case InstructionCode.Multiply:
                    PerformIntOrFloatOp((a, b) => a * b, (a, b) => a * b);
                    break;

                case InstructionCode.Divide:
                    PerformDivide();
                    break;

                case InstructionCode.Negate:
                    PerformNegate();
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
                            _exitCode = finalVal.AsLong();
                        }
                        else if (finalVal.IsFloat())
                        {
                            _exitCode = (long)finalVal.AsDecimal();
                        }
                    }

                    return _result ?? new Value(0L);

                default:
                    throw new NotImplementedException($"Instruction {instruction.Code} is not supported");
            }
        }
    }

    private void PerformAdd()
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        if (a.IsString() && b.IsString())
        {
            _evaluationStack.Push(new Value(a.AsString() + b.AsString()));
        }
        else if (a.IsInt() && b.IsInt())
        {
            _evaluationStack.Push(new Value(checked(a.AsLong() + b.AsLong())));
        }
        else
        {
            _evaluationStack.Push(new Value(a.AsDecimal() + b.AsDecimal()));
        }
    }

    private void PerformDivide()
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        if (a.IsInt() && b.IsInt())
        {
            long divisor = b.AsLong();
            if (divisor == 0)
            {
                throw new InvalidOperationException("Division by zero");
            }

            _evaluationStack.Push(new Value(a.AsLong() / divisor));
        }
        else
        {
            decimal divisor = b.AsDecimal();
            if (divisor == 0)
            {
                throw new InvalidOperationException("Division by zero");
            }

            _evaluationStack.Push(new Value(a.AsDecimal() / divisor));
        }
    }

    private void PerformNegate()
    {
        Value a = _evaluationStack.Pop();
        if (a.IsInt())
        {
            _evaluationStack.Push(new Value(-a.AsLong()));
        }
        else
        {
            _evaluationStack.Push(new Value(-a.AsDecimal()));
        }
    }

    private void PerformIntOrFloatOp(
        Func<long, long, long> intOp,
        Func<decimal, decimal, decimal> floatOp)
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        if (a.IsInt() && b.IsInt())
        {
            _evaluationStack.Push(new Value(intOp(a.AsLong(), b.AsLong())));
        }
        else
        {
            _evaluationStack.Push(new Value(floatOp(a.AsDecimal(), b.AsDecimal())));
        }
    }

    private void CallBuiltin(Value operand)
    {
        if (!operand.IsInt())
        {
            throw new InvalidOperationException(
                $"VM Error: Builtin function operand must be an integer (code), but found {operand.ToString()}");
        }

        BuiltinFunctionCode code = (BuiltinFunctionCode)operand.AsLong();

        switch (code)
        {
            case BuiltinFunctionCode.Print:
                _builtinFunctions.Print(_evaluationStack.Pop());
                break;

            default:
                throw new NotImplementedException($"Builtin function with code '{code}' is not supported");
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