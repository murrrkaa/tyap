using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Runtime;
using PsTiger.Semantics.Exceptions;
using PsTiger.Semantics.Helpers;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Semantics.Passes;

/// <summary>
/// Проход по AST для проверки корректности программы с точки зрения совместимости типов данных.
/// </summary>
/// <exception cref="TypeErrorException">Бросается при несоответствии типов данных в процессе проверки.</exception>
public class CheckTypesPass : AbstractPass
{
    /// <summary>
    /// Проверяет соответствие типов параметров функции и аргументов при вызове этой функции.
    /// </summary>
    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        CheckFunctionArgumentTypes(e, e.Function);
    }

    public override void Visit(FunctionDeclaration d)
    {
        base.Visit(d);
        CheckAreSameTypes("function body", d.Body, d.ResultType);
    }

    /// <summary>
    /// Проверяет тип переменной и тип выражения, которым она инициализируется.
    /// </summary>
    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);

        ValueType inferredType = d.InitialValue.ResultType;

        if (d.ResolvedDeclaredType != ValueType.Void)
        {
            if (!ValueTypeUtil.AreCompatibleTypes(d.ResolvedDeclaredType, inferredType))
            {
                throw new TypeErrorException($"Cannot initialize {d.ResolvedDeclaredType} with {inferredType}");
            }
        }
    }

    public override void Visit(AssignmentExpression e)
    {
        base.Visit(e);
        if (!ValueTypeUtil.AreCompatibleTypes(e.Left.ResultType, e.Right.ResultType))
        {
            throw new TypeErrorException(
                $"Cannot assign value of type {e.Right.ResultType} to variable of type {e.Left.ResultType}"
            );
        }
    }

    public override void Visit(IfStatement e)
    {
        s.Condition.Accept(this);
        s.ThenBranch.Accept(this);
        s.ElseBranch?.Accept(this);

        //условие д быть логическим
        if (s.Condition.ResultType != ValueType.Bool)
        {
            throw new TypeErrorException($"Condition in 'if' must be bool, but got {s.Condition.ResultType}");
        }
    }

    public override void Visit(WhileLoopExpression e)
    {
        base.Visit(e);

        CheckAreSameTypes("while loop condition", e.Condition, ValueType.Bool);
    }

    public override void Visit(ForLoopExpression e)
    {
        base.Visit(e);

        CheckAreSameTypes("for loop start value", e.StartValue, ValueType.Int);
        CheckAreSameTypes("for loop end value", e.EndValue, ValueType.Int);
        CheckAreSameTypes("for loop body", e.LoopBody, ValueType.Void);
    }

    /// <summary>
    /// Проверяет соответствие типов формальных параметров и фактических параметров (аргументов) при вызове функции.
    /// </summary>
    private static void CheckFunctionArgumentTypes(FunctionCallExpression e, AbstractFunctionDeclaration function)
    {
        // Для каждого i-го аргумента выводим тип и сверяем с типом i-го параметра функции.
        for (int i = 0, iMax = e.Arguments.Count; i < iMax; ++i)
        {
            Expression argument = e.Arguments[i];
            AbstractParameterDeclaration parameter = function.Parameters[i];
            if (!ValueTypeUtil.AreCompatibleTypes(argument.ResultType, parameter.ResultType))
            {
                throw new TypeErrorException(
                    $"Cannot apply argument #{i} of type {argument.ResultType} to function {e.Name} parameter {parameter.Name} which has type {parameter.ResultType}"
                );
            }
        }
    }

    private static void CheckAreSameTypes(string category, Expression expression, ValueType expectedType)
    {
        if (!ValueTypeUtil.AreCompatibleTypes(expression.ResultType, expectedType))
        {
            throw new TypeErrorException(category, expectedType, expression.ResultType);
        }
    }

    private static void CheckAreCompatibleTypes(string category, Expression expression, ValueType expectedType)
    {
        if (!ValueTypeUtil.AreCompatibleTypes(expression.ResultType, expectedType))
        {
            throw new TypeErrorException(category, expectedType, expression.ResultType);
        }
    }
}