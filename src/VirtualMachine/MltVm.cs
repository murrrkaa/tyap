using System;
using System.Collections.Generic;
using System.Globalization;

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

                case InstructionCode.And:
                    {
                        Value b = _evaluationStack.Pop();
                        Value a = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(a.AsBool() && b.AsBool()));
                        break;
                    }
                case InstructionCode.Or:
                    {
                        Value b = _evaluationStack.Pop();
                        Value a = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(a.AsBool() || b.AsBool()));
                        break;
                    }

                case InstructionCode.Equal:
                    PerformComparison((a, b) => a.Equals(b));
                    break;

                case InstructionCode.NotEqual:
                    PerformComparison((a, b) => !a.Equals(b));
                    break;

                case InstructionCode.Less:
                    PerformOrderedComparison((a, b) => a < b, (a, b) => a < b);
                    break;

                case InstructionCode.LessOrEqual:
                    PerformOrderedComparison((a, b) => a <= b, (a, b) => a <= b);
                    break;

                case InstructionCode.GreaterThan:
                    PerformOrderedComparison((a, b) => a > b, (a, b) => a > b);
                    break;

                case InstructionCode.GreaterThanOrEqual:
                    PerformOrderedComparison((a, b) => a >= b, (a, b) => a >= b);
                    break;

                case InstructionCode.Not:
                    _evaluationStack.Push(new Value(!_evaluationStack.Pop().AsBool()));
                    break;

                case InstructionCode.JumpIfFalse:
                    if (!_evaluationStack.Peek().AsBool())
                        _instructionPointer = (int)instruction.Operand!.AsLong();
                    break;

                case InstructionCode.JumpIfTrue:
                    if (_evaluationStack.Peek().AsBool())
                        _instructionPointer = (int)instruction.Operand!.AsLong();
                    break;

                case InstructionCode.Jump:
                    _instructionPointer = (int)instruction.Operand!.AsLong();
                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin((BuiltinFunctionCode)instruction.Operand!.AsLong());
                    break;

                case InstructionCode.Halt:
                    if (_evaluationStack.Count > 0)
                    {
                        Value finalVal = _evaluationStack.Pop();
                        if (finalVal.IsInt()) _exitCode = finalVal.AsLong();
                        else if (finalVal.IsFloat()) _exitCode = (long)finalVal.AsDecimal();
                    }
                    return _result ?? new Value(0L);

                default:
                    throw new NotImplementedException(
                        $"Instruction {instruction.Code} is not supported");
            }
        }
    }

    private void CallBuiltin(BuiltinFunctionCode code)
    {
        switch (code)
        {
            case BuiltinFunctionCode.Print:
                _builtinFunctions.Print(_evaluationStack.Pop());
                break;

            case BuiltinFunctionCode.ReadInt:
                _evaluationStack.Push(new Value(_builtinFunctions.ReadInt()));
                break;

            case BuiltinFunctionCode.ReadFloat:
                _evaluationStack.Push(new Value(_builtinFunctions.ReadFloat()));
                break;

            case BuiltinFunctionCode.ReadString:
                _evaluationStack.Push(new Value(_builtinFunctions.ReadString()));
                break;

            case BuiltinFunctionCode.Len:
                _evaluationStack.Push(new Value((long)_evaluationStack.Pop().AsString().Length));
                break;

            case BuiltinFunctionCode.Substring:
                {
                    long count = _evaluationStack.Pop().AsLong();
                    long start = _evaluationStack.Pop().AsLong();
                    string s = _evaluationStack.Pop().AsString();
                    _evaluationStack.Push(new Value(s.Substring((int)start, (int)count)));
                    break;
                }

            case BuiltinFunctionCode.ToString:
                _evaluationStack.Push(new Value(_evaluationStack.Pop().ToString()));
                break;

            case BuiltinFunctionCode.ParseInt:
                {
                    string s = _evaluationStack.Pop().AsString();
                    if (!long.TryParse(s, out long result))
                        throw new InvalidOperationException($"Cannot parse '{s}' as int");
                    _evaluationStack.Push(new Value(result));
                    break;
                }

            case BuiltinFunctionCode.ToBool:
                _evaluationStack.Push(new Value(_evaluationStack.Pop().AsLong() != 0));
                break;

            case BuiltinFunctionCode.ToFloat:
                _evaluationStack.Push(new Value((decimal)_evaluationStack.Pop().AsLong()));
                break;

            default:
                throw new NotImplementedException($"Builtin {code} is not supported");
        }
    }

    private void PerformAdd()
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        if (a.IsString() && b.IsString())
            _evaluationStack.Push(new Value(a.AsString() + b.AsString()));
        else if (a.IsInt() && b.IsInt())
            _evaluationStack.Push(new Value(checked(a.AsLong() + b.AsLong())));
        else
            _evaluationStack.Push(new Value(a.AsDecimal() + b.AsDecimal()));
    }

    private void PerformDivide()
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        if (a.IsInt() && b.IsInt())
        {
            long divisor = b.AsLong();
            if (divisor == 0) throw new InvalidOperationException("Division by zero");
            _evaluationStack.Push(new Value(a.AsLong() / divisor));
        }
        else
        {
            decimal divisor = b.AsDecimal();
            if (divisor == 0) throw new InvalidOperationException("Division by zero");
            _evaluationStack.Push(new Value(a.AsDecimal() / divisor));
        }
    }

    private void PerformComparison(Func<Value, Value, bool> predicate)
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();
        _evaluationStack.Push(new Value(predicate(a, b)));
    }

    private void PerformOrderedComparison(
        Func<long, long, bool> intOp,
        Func<decimal, decimal, bool> floatOp)
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        bool result = (a.IsInt() && b.IsInt())
            ? intOp(a.AsLong(), b.AsLong())
            : floatOp(a.AsDecimal(), b.AsDecimal());

        _evaluationStack.Push(new Value(result));
    }

    private void PerformIntOrFloatOp(
        Func<long, long, long> intOp,
        Func<decimal, decimal, decimal> floatOp)
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        if (a.IsInt() && b.IsInt())
            _evaluationStack.Push(new Value(intOp(a.AsLong(), b.AsLong())));
        else
            _evaluationStack.Push(new Value(floatOp(a.AsDecimal(), b.AsDecimal())));
    }

    private static void ValidateInstructions(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0)
            throw new InvalidOperationException("Empty program");

        if (instructions[instructions.Count - 1].Code != InstructionCode.Halt)
            throw new InvalidOperationException("Program must end with Halt");
    }
}