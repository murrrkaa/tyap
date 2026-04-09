using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Runtime;
using PsTiger.Semantics.Exceptions;
using PsTiger.Semantics.Helpers;
using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Semantics.Passes;

public class CheckTypesPass : AbstractPass
{

    private FunctionDeclaration? _currentFunction;

    public override void Visit(Program node)
    {
        base.Visit(node);

        if (node.MainFunction.ResolvedReturnType != ValueType.Int)
        {
            throw new TypeErrorException("Function main must return type int");
        }

        if (!HasReturnInBlock(node.MainFunction.Body))
        {
            throw new TypeErrorException("Function main must contain a return statement");
        }
    }

    public override void Visit(FunctionDeclaration d)
    {
        FunctionDeclaration? previous = _currentFunction;
        _currentFunction = d;

        base.Visit(d);

        if (d.ResolvedReturnType != ValueType.Void && !HasReturnInBlock(d.Body))
        {
            throw new TypeErrorException($"Function {d.Name} must contain a return statement");
        }

        _currentFunction = previous;
    }

    public override void Visit(ReturnStatement e)
    {
        base.Visit(e);

        if (_currentFunction != null)
        {
            if (e.Expression != null)
            {
                if (!ValueTypeUtil.AreCompatibleTypes(e.Expression.ResultType, _currentFunction.ResolvedReturnType))
                {
                    throw new TypeErrorException(
                        $"Return type {e.Expression.ResultType} does not match function {_currentFunction.Name} return type {_currentFunction.ResolvedReturnType}"
                    );
                }
            }
            else if (_currentFunction.ResolvedReturnType != ValueType.Void)
            {
                throw new TypeErrorException(
                    $"Function {_currentFunction.Name} expects return value of type {_currentFunction.ResolvedReturnType}"
                );
            }
        }
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        CheckFunctionArgumentTypes(e, e.Function);
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        if (d.InitialValue.ResultType == ValueType.Void)
        { 
            throw new TypeErrorException("Cannot initialize variable from void expression"); 
        }

        if (!ValueTypeUtil.AreCompatibleTypes(d.ResolvedType, d.InitialValue.ResultType))
        { 
            throw new TypeErrorException($"Type mismatch: expected {d.ResolvedType}, got {d.InitialValue.ResultType}"); 
        }
    }

    public override void Visit(ConstantDeclaration d)
    {
        base.Visit(d);
        if (d.InitialValue.ResultType == ValueType.Void)
        { 
            throw new TypeErrorException("Cannot initialize constant from void expression"); 
        }

        if (!ValueTypeUtil.AreCompatibleTypes(d.ResolvedType, d.InitialValue.ResultType))
        { 
            throw new TypeErrorException($"Type mismatch: expected {d.ResolvedType}, got {d.InitialValue.ResultType}"); 
        }
    }

    public override void Visit(AssignmentStatement node)
    {
        base.Visit(node);
    }

    public override void Visit(PrintStatement node)
    {
        base.Visit(node);
        foreach (Expression arg in node.Arguments)
        {
            if (arg.ResultType == ValueType.Void)
            {
                throw new TypeErrorException("Cannot print void expression");
            }
        }
    }

    public override void Visit(IfStatement node)
    {
        base.Visit(node);
        if (node.Condition.ResultType != ValueType.Bool)
        { 
            throw new TypeErrorException("Condition must be of type bool"); 
        }
    }

    public override void Visit(WhileStatement node)
    {
        base.Visit(node);
        if (node.Condition.ResultType != ValueType.Bool)
        { 
            throw new TypeErrorException("Condition must be of type bool"); 
        }
    }

    public override void Visit(ForStatement node)
    {
        base.Visit(node);
        if (node.Condition.ResultType != ValueType.Bool)
        { 
                throw new TypeErrorException("Condition must be of type bool"); 
        }
    }

    private bool HasReturnInBlock(BlockStatement block)
    {
        foreach (AstNode node in block.Nodes)
        {
            if (node is ReturnStatement)
            {
                return true;
            }
            if (node is BlockStatement innerBlock && HasReturnInBlock(innerBlock))
            {
                return true;
            }
        }

        return false;
    }

    private static void CheckFunctionArgumentTypes(FunctionCallExpression e, AbstractFunctionDeclaration function)
    {
        for (int i = 0, iMax = e.Arguments.Count; i < iMax; ++i)
        {
            Expression argument = e.Arguments[i];
            AbstractParameterDeclaration parameter = function.Parameters[i];

            ValueType paramType = parameter switch
            {
                BuiltinFunctionParameter builtin => builtin.Type,
                ParameterDeclaration user => user.ResolvedType,
                _ => ValueType.Void
            };

            if (!ValueTypeUtil.AreCompatibleTypes(argument.ResultType, paramType))
            {
                throw new TypeErrorException(
                    $"Argument #{i} type {argument.ResultType} does not match parameter {parameter.Name} type {paramType}"
                );
            }
        }
    }
}