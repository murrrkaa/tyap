using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Runtime;
using PsTiger.Semantics.Exceptions;
using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Semantics.Passes;

public sealed class CheckContextSensitiveRulesPass : AbstractPass
{
    private readonly Stack<ExpressionContext> _contextStack = [];

    public CheckContextSensitiveRulesPass()
    {
        _contextStack.Push(ExpressionContext.Default);
    }

    private enum ExpressionContext
    {
        Default,
        InsideLoop,
    }

    public override void Visit(Program node) => base.Visit(node);

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        if (e.Arguments.Count != e.Function.Parameters.Count)
        {
            throw new InvalidFunctionCallException(
                $"Function {e.Name} requires {e.Function.Parameters.Count} arguments, got {e.Arguments.Count}"
            );
        }
    }

    public override void Visit(AssignmentStatement node)
    {
        base.Visit(node);

        if (node.Value.ResultType == ValueType.Void)
        {
            throw new InvalidAssignmentException("Cannot assign a void expression to a variable");
        }
    }

    public override void Visit(IfStatement node)
    {
        base.Visit(node);
        if (node.Condition.ResultType != ValueType.Bool)
        {
            throw new InvalidExpressionException("Condition in if statement must be of type bool");
        }
    }

    public override void Visit(WhileStatement node)
    {
        _contextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(node);

            if (node.Condition.ResultType != ValueType.Bool)
            {
                throw new InvalidExpressionException("Condition in while loop must be of type bool");
            }
        }
        finally
        {
            _contextStack.Pop();
        }
    }

    public override void Visit(ForStatement node)
    {
        _contextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(node);
            if (node.Condition.ResultType != ValueType.Bool)
            {
                throw new InvalidExpressionException("Condition in for loop must be of type bool");
            }
        }
        finally
        {
            _contextStack.Pop();
        }
    }

    public override void Visit(BreakStatement node)
    {
        base.Visit(node);
        if (_contextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"break\" statement is allowed only inside a loop");
        }
    }

    public override void Visit(ContinueStatement node)
    {
        base.Visit(node);
        if (_contextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"continue\" statement is allowed only inside a loop");
        }
    }

    public override void Visit(FunctionDeclaration d)
    {
        _contextStack.Push(ExpressionContext.Default);
        try { base.Visit(d); }
        finally { _contextStack.Pop(); }
    }
}