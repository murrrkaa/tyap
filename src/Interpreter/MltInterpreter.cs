using System;
using System.Collections.Generic;

using Mlt.Parsing;
using Mlt.Semantics;
using Mlt.VirtualMachine;
using Mlt.VirtualMachine.Instructions;
using Mlt.VirtualMachineCodegen;
using Mlt.Semantics;

using AstProgram = Mlt.Ast.Program;

namespace Mlt.Interpreter;

public class MltInterpreter
{
    private readonly IEnvironment _environment;
    private int _exitCode;

    public MltInterpreter(IEnvironment environment)
    {
        _environment = environment;
    }

    public int ExitCode => _exitCode;

    public int Execute(string code)
    {
        Parser parser = new(code);
        AstProgram program = parser.ParseProgram();

        SemanticsChecker semantics = new();
        semantics.Check(program);

        MltVmCodegen codegen = new();
        List<Instruction> instructionsList = codegen.GenerateCode(program);
        IReadOnlyList<Instruction> instructions = instructionsList;

        MltVm vm = new(_environment, instructions);
        vm.RunProgram();

        _exitCode = vm.ExitCode;
        return _exitCode;
    }
}