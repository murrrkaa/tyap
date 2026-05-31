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
    private readonly Stack<Dictionary<string, Value>> _variableScopes = new();
    private readonly Stack<int> _callStack = new();

    public MltVm(IEnvironment environment, IReadOnlyList<Instruction> instructions)
    {
        ValidateInstructions(instructions);
        _builtinFunctions = new BuiltinFunctions(environment);
        _instructions = instructions;
        _instructionPointer = 0;
        _exitCode = 0;
        _evaluationStack = new Stack<Value>();
        _variableScopes.Push(new Dictionary<string, Value>());
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

                case InstructionCode.PushVars:
                    _variableScopes.Push(new Dictionary<string, Value>());
                    break;

                case InstructionCode.PopVars:
                    {
                        if (_variableScopes.Count > 1)
                        {
                            _variableScopes.Pop();
                        }

                        break;
                    }

                case InstructionCode.DefineVar:
                    _variableScopes.Peek()[instruction.Operand!.AsString()] =
                        _evaluationStack.Pop();
                    break;

                case InstructionCode.StoreVar:
                    {
                        string varName = instruction.Operand!.AsString();
                        Value value = _evaluationStack.Pop();
                        StoreVariable(varName, value);
                        break;
                    }

                case InstructionCode.LoadVar:
                    _evaluationStack.Push(LoadVariable(instruction.Operand!.AsString()));
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
                    {
                        _instructionPointer = (int)instruction.Operand!.AsLong();
                    }

                    break;

                case InstructionCode.JumpIfTrue:
                    if (_evaluationStack.Peek().AsBool())
                    {
                        _instructionPointer = (int)instruction.Operand!.AsLong();
                    }

                    break;

                case InstructionCode.Jump:
                    _instructionPointer = (int)instruction.Operand!.AsLong();
                    break;

                case InstructionCode.Call:
                    _callStack.Push(_instructionPointer);
                    _instructionPointer = (int)instruction.Operand!.AsLong();
                    break;

                case InstructionCode.Return:
                    _instructionPointer = _callStack.Pop();
                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin((BuiltinFunctionCode)instruction.Operand!.AsLong());
                    break;

                case InstructionCode.Halt:
                    {
                        if (_evaluationStack.Count > 0)
                        {
                            Value result = _evaluationStack.Pop();

                            if (result.IsInt())
                            {
                                _exitCode = result.AsLong();
                            }
                            else if (result.IsFloat())
                            {
                                _exitCode = (long)result.AsDouble();
                            }

                            return result;
                        }

                        _exitCode = 0;
                        return new Value(0L);
                    }

                default:
                    throw new NotImplementedException(
                        $"Instruction {instruction.Code} is not supported");
            }
        }
    }

    private void StoreVariable(string name, Value value)
    {
        foreach (Dictionary<string, Value> scope in _variableScopes)
        {
            if (scope.ContainsKey(name))
            {
                scope[name] = value;
                return;
            }
        }

        throw new InvalidOperationException($"Переменная '{name}' не объявлена");
    }

    private Value LoadVariable(string name)
    {
        foreach (Dictionary<string, Value> scope in _variableScopes)
        {
            if (scope.TryGetValue(name, out Value? value))
            {
                return value;
            }
        }

        throw new InvalidOperationException($"Переменная '{name}' не объявлена");
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
                _evaluationStack.Push(
                    new Value((long)_evaluationStack.Pop().AsString().Length));
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
                    {
                        throw new InvalidOperationException($"Невозможно преобразовать '{s}' в int");
                    }

                    _evaluationStack.Push(new Value(result));
                    break;
                }

            case BuiltinFunctionCode.ToBool:
                _evaluationStack.Push(new Value(_evaluationStack.Pop().AsLong() != 0));
                break;

            case BuiltinFunctionCode.ToFloat:
                _evaluationStack.Push(new Value((double)_evaluationStack.Pop().AsLong()));
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
        {
            _evaluationStack.Push(new Value(a.AsString() + b.AsString()));
        }
        else if (a.IsInt() && b.IsInt())
        {
            _evaluationStack.Push(new Value(checked(a.AsLong() + b.AsLong())));
        }
        else
        {
            _evaluationStack.Push(new Value(a.AsDouble() + b.AsDouble()));
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
                throw new InvalidOperationException("Деление на ноль");
            }

            _evaluationStack.Push(new Value(a.AsLong() / divisor));
        }
        else
        {
            double divisor = b.AsDouble();
            if (divisor == 0)
            {
                throw new InvalidOperationException("Деление на ноль");
            }

            _evaluationStack.Push(new Value(a.AsDouble() / divisor));
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
        Func<double, double, bool> floatOp)
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        bool result = (a.IsInt() && b.IsInt())
            ? intOp(a.AsLong(), b.AsLong())
            : floatOp(a.AsDouble(), b.AsDouble());

        _evaluationStack.Push(new Value(result));
    }

    private void PerformIntOrFloatOp(
        Func<long, long, long> intOp,
        Func<double, double, double> floatOp)
    {
        Value b = _evaluationStack.Pop();
        Value a = _evaluationStack.Pop();

        if (a.IsInt() && b.IsInt())
        {
            _evaluationStack.Push(new Value(intOp(a.AsLong(), b.AsLong())));
        }
        else
        {
            _evaluationStack.Push(new Value(floatOp(a.AsDouble(), b.AsDouble())));
        }
    }

    private static void ValidateInstructions(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0)
        {
            throw new InvalidOperationException("Программа не содержит инструкций");
        }

        if (instructions[instructions.Count - 1].Code != InstructionCode.Halt)
        {
            throw new InvalidOperationException("Программа должна завершаться инструкцией Halt");
        }
    }
}