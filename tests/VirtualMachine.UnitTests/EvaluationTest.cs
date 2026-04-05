using PsTiger.Tests.TestLibrary.TestDoubles;
using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Builtins;
using PsTiger.VirtualMachine.Instructions;
using System.Collections.Generic;

namespace PsTiger.VirtualMachine.UnitTests;

public class EvaluationTest
{
    [Theory]
    [MemberData(nameof(GetEvaluateExpressionData))]
    public void Can_evaluate_expression(
        List<Instruction> program,
        string expectedOutput)
    {
        FakeEnvironment environment = new();
        TigerVm vm = new(environment, program);

        vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expectedOutput, environment.BufferedOutput);
        Assert.Empty(environment.FlushedOutput);
    }

    public static TheoryData<List<Instruction>, string> GetEvaluateExpressionData()
    {
        return new()
        {
            // (20 + 50) - 3 = 67
            {
                [
                    new Instruction(InstructionCode.Push, 20),
                    new Instruction(InstructionCode.Push, 50),
                    new Instruction(InstructionCode.Add),
                    new Instruction(InstructionCode.Push, 3),
                    new Instruction(InstructionCode.Subtract),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintI),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "67"
            },

            // (20 * 50) / -5 = -200
            {
                [
                    new Instruction(InstructionCode.Push, 20),
                    new Instruction(InstructionCode.Push, 50),
                    new Instruction(InstructionCode.Multiply),
                    new Instruction(InstructionCode.Push, -5),
                    new Instruction(InstructionCode.Divide),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintI),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "-200"
            },

            // 1 & 0 | 1 = 1
            {
                [
                    new Instruction(InstructionCode.Push, 1),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.And),
                    new Instruction(InstructionCode.Push, 1),
                    new Instruction(InstructionCode.Or),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintI),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "1"
            },

            // 17 < 20 = 1
            {
                [
                    new Instruction(InstructionCode.Push, 17),
                    new Instruction(InstructionCode.Push, 20),
                    new Instruction(InstructionCode.Less),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintI),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "1"
            },

            // unary minus
            {
                [
                    new Instruction(InstructionCode.Push, 1024),
                    new Instruction(InstructionCode.Negate),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintI),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "-1024"
            },

            // pop test
            {
                [
                    new Instruction(InstructionCode.Push, 1024),
                    new Instruction(InstructionCode.Push, 702),
                    new Instruction(InstructionCode.Pop),
                    new Instruction(InstructionCode.Negate),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintI),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "-1024"
            }
        };
    }
}