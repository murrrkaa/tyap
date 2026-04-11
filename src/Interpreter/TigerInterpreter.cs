using System;
using System.Collections.Generic;

using PsTiger.Parsing;
using PsTiger.VirtualMachine;
using PsTiger.VirtualMachine.Instructions;
using PsTiger.VirtualMachineCodegen;

using AstProgram = PsTiger.Ast.Program;

namespace PsTiger.Interpreter;

public class TigerInterpreter
{
    private readonly IEnvironment _environment;
    private int _exitCode;

    public TigerInterpreter(IEnvironment environment)
    {
        _environment = environment;
    }

    public int ExitCode => _exitCode;

    public int Execute(string code)
    {
        Parser parser = new(code);
        AstProgram program = parser.ParseProgram();

        TigerVmCodegen codegen = new();
        List<Instruction> instructionsList = codegen.GenerateCode(program);

        IReadOnlyList<Instruction> instructions = instructionsList;

        TigerVm vm = new(_environment, instructions);
        vm.RunProgram();

        _exitCode = vm.ExitCode;
        return _exitCode;
    }
}