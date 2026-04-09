using PsTiger.Runtime;
using PsTiger.VirtualMachine.Builtins;
using PsTiger.VirtualMachine.Instructions;

namespace PsTiger.VirtualMachine;

public class TigerVm
{
    private readonly BuiltinFunctions _builtinFunctions;
    private readonly IReadOnlyList<Instruction> _instructions;
    private int _instructionPointer;
    private int _exitCode;
    private readonly Stack<Value> _evaluationStack;
    private VariablesTable? _variables;
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
        _variables = new VariablesTable();
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
                    _evaluationStack.Pop();
                    break;

                case InstructionCode.StoreVar:
                    {
                        Value value = _evaluationStack.Pop();
                        string variableName = instruction.Operand.AsString();
                        _variables!.AssignVariable(variableName, value);
                    }
                    break;

                case InstructionCode.DefineVar:
                    {
                        Value value = _evaluationStack.Pop();
                        string variableName = instruction.Operand.AsString();
                        _variables!.DefineVariable(variableName, value);
                    }
                    break;

                case InstructionCode.LoadVar:
                    {
                        string variableName = instruction.Operand.AsString();
                        Value value = _variables!.GetVariable(variableName);
                        _evaluationStack.Push(value);
                    }
                    break;

                case InstructionCode.Add:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();

                        if (left.IsString() && right.IsString())
                        {
                            string result = left.AsString() + right.AsString();
                            _evaluationStack.Push(new Value(result));
                        }

                        else if (left.IsInt() && right.IsInt())
                        {
                            _evaluationStack.Push(new Value(left.AsInt() + right.AsInt()));
                        }

                        else if (left.IsFloat() || right.IsFloat())
                        {
                            double l = left.IsFloat() ? left.AsFloat() : left.AsInt();
                            double r = right.IsFloat() ? right.AsFloat() : right.AsInt();
                            _evaluationStack.Push(new Value(l + r));
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"Cannot add values of types {left} and {right}"
                            );
                        }
                    }
                    break;

                case InstructionCode.Subtract:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.AsInt() - right.AsInt()));
                    }
                    break;

                case InstructionCode.Multiply:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.AsInt() * right.AsInt()));
                    }
                    break;

                case InstructionCode.Divide:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.AsInt() / right.AsInt()));
                    }
                    break;

                case InstructionCode.And:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value((left.AsInt() != 0 && right.AsInt() != 0) ? 1 : 0));
                    }
                    break;

                case InstructionCode.Or:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value((left.AsInt() != 0 || right.AsInt() != 0) ? 1 : 0));
                    }
                    break;

                case InstructionCode.Not:
                    {
                        Value operand = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(operand.AsInt() == 0 ? 1 : 0));
                    }
                    break;

                case InstructionCode.Equal:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.Equals(right) ? 1 : 0));
                    }
                    break;

                case InstructionCode.NotEqual:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.Equals(right) ? 0 : 1));
                    }
                    break;

                case InstructionCode.Less:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.LessThan(right) ? 1 : 0));
                    }
                    break;

                case InstructionCode.LessOrEqual:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.LessThanOrEqual(right) ? 1 : 0));
                    }
                    break;

                case InstructionCode.GreaterThan:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.GreaterThan(right) ? 1 : 0));
                    }
                    break;

                case InstructionCode.GreaterThanOrEqual:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.GreaterThanOrEqual(right) ? 1 : 0));
                    }
                    break;

                case InstructionCode.Jump:
                    _instructionPointer = instruction.Operand.AsInt();
                    break;

                case InstructionCode.JumpIfTrue:
                    {
                        Value condition = _evaluationStack.Pop();
                        if (condition.AsInt() != 0) _instructionPointer = instruction.Operand.AsInt();
                    }
                    break;

                case InstructionCode.JumpIfFalse:
                    {
                        Value condition = _evaluationStack.Pop();
                        if (condition.AsInt() == 0) _instructionPointer = instruction.Operand.AsInt();
                    }
                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin((BuiltinFunctionCode)instruction.Operand.AsInt());
                    break;

                case InstructionCode.Call:
                    {
                        _returnStack.Push(new ReturnContext(_instructionPointer, _variables));
                        _instructionPointer = instruction.Operand.AsInt();
                    }
                    break;

                // ✅ ИСПРАВЛЕНО: Корректная обработка возврата из main и вложенных функций
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

                            if (returnValue.IsInt())
                            {
                                _exitCode = returnValue.AsInt();
                            }
                            else if (returnValue.IsBool())
                            {
                                _exitCode = returnValue.AsBool() ? 1 : 0;
                            }

                            return _result;
                        }
                        else
                        { 
                            if (returnValue != Value.Void)
                            {
                                _evaluationStack.Push(returnValue);
                            }

                            ReturnContext context = _returnStack.Pop();
                            _instructionPointer = context.InstructionPointer;
                            _variables = context.Variables;
                        }
                    }
                    break;

                case InstructionCode.StoreResult:
                    _result = _evaluationStack.Pop();
                    break;

                case InstructionCode.Halt:
                    if (_evaluationStack.Count > 0)
                    {
                        _exitCode = _evaluationStack.Pop().AsInt();
                    }
                    return _result;

                case InstructionCode.PushVars:
                    {
                        int variableTableDepth = instruction.Operand.AsInt();
                        VariablesTable? parentTable = (variableTableDepth != 0)
                            ? _variables!.GetAncestor(variableTableDepth)
                            : null;
                        _variables = new VariablesTable(parentTable);
                    }
                    break;

                case InstructionCode.PopVars:
                    _variables = _variables!.Parent;
                    break;

                default:
                    throw new NotImplementedException($"Unsupported instruction code: {instruction.Code}");
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
                _evaluationStack.Push(_builtinFunctions.ReadInt());
                break;

            case BuiltinFunctionCode.ReadFloat:
                _evaluationStack.Push(_builtinFunctions.ReadFloat());
                break;

            case BuiltinFunctionCode.ReadString:
                _evaluationStack.Push(_builtinFunctions.ReadString());
                break;

            case BuiltinFunctionCode.Len:
                _evaluationStack.Push(_builtinFunctions.Len(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.Substring:
                {
                    Value count = _evaluationStack.Pop();
                    Value start = _evaluationStack.Pop();
                    Value value = _evaluationStack.Pop();
                    _evaluationStack.Push(_builtinFunctions.Substring(value, start, count));
                }
                break;

            case BuiltinFunctionCode.ToString:
                _evaluationStack.Push(_builtinFunctions.ToString(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.ParseInt:
                _evaluationStack.Push(_builtinFunctions.ParseInt(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.ToBool:
                _evaluationStack.Push(_builtinFunctions.ToBool(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.ToFloat:
                _evaluationStack.Push(_builtinFunctions.ToFloat(_evaluationStack.Pop()));
                break;

            default:
                throw new ArgumentException($"Unknown builtin function: {code}");
        }
    }

    private static void ValidateInstructions(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0)
        {
            throw new InvalidOperationException("Invalid empty VM program");
        }

        InstructionCode lastInstructionCode = instructions[^1].Code;
        if (lastInstructionCode != InstructionCode.Halt
            && lastInstructionCode != InstructionCode.Return
            && lastInstructionCode != InstructionCode.Jump)
        {
            throw new InvalidOperationException(
                $"Last instruction must be {InstructionCode.Halt}, " +
                $"{InstructionCode.Return} or {InstructionCode.Jump}, got {lastInstructionCode}"
            );
        }
    }

    private record struct ReturnContext(
        int InstructionPointer,
        VariablesTable? Variables
    );
}