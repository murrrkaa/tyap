using PsTiger.Ast;
using PsTiger.Ast.Declarations;
using PsTiger.Ast.Expressions;
using PsTiger.Ast.Statements;
using PsTiger.Runtime;
using PsTiger.Semantics.Exceptions;
using PsTiger.Semantics.Helpers;

using ValueType = PsTiger.Runtime.ValueType;

namespace PsTiger.Semantics.Passes;

public sealed class ResolveTypesPass : AbstractPass
{
    public override void Visit(Program node)
    {
        base.Visit(node);
    }

    public override void Visit(LiteralExpression e)
    {
        base.Visit(e);
        e.ResultType = e.Type;
    }

    public override void Visit(BinaryOperationExpression e)
    {
        base.Visit(e);
        ValueType? resultType = GetBinaryOperationResultType(e.Operation, e.Left.ResultType, e.Right.ResultType);
        if (resultType is null)
            throw new TypeErrorException($"Operator '{e.Operation}' cannot be applied to types {e.Left.ResultType} and {e.Right.ResultType}");
        e.ResultType = resultType;
    }

    public override void Visit(UnaryNotExpression e)
    {
        base.Visit(e);
        if (e.Operand.ResultType != ValueType.Bool)
            throw new TypeErrorException($"Operator '!' requires bool, got {e.Operand.ResultType}");
        e.ResultType = ValueType.Bool;
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        if (e.Function is FunctionDeclaration userFunc)
            e.ResultType = userFunc.ResolvedReturnType;
        else if (e.Function is BuiltinFunction builtinFunc)
            e.ResultType = builtinFunc.ResultType;
        else
            e.ResultType = ValueType.Void;
    }

    public override void Visit(VariableAccessExpression e)
    {
        base.Visit(e);
        if (e.Variable is VariableDeclaration varDecl)
            e.ResultType = varDecl.ResolvedType;
        else if (e.Variable is ParameterDeclaration paramDecl)
            e.ResultType = paramDecl.ResolvedType;
        else
            e.ResultType = ValueType.Void;
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        d.ResolvedType = ParseValueType(d.DeclaredTypeName);
    }

    public override void Visit(ConstantDeclaration d)
    {
        base.Visit(d);
        d.ResolvedType = ParseValueType(d.DeclaredTypeName);
    }

    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);
        d.ResolvedType = ParseValueType(d.TypeName);
    }

    public override void Visit(FunctionDeclaration d)
    {
        d.ResolvedReturnType = ParseValueType(d.DeclaredReturnTypeName);
        base.Visit(d);
    }

    private static ValueType ParseValueType(string? typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return ValueType.Void;
        }

        return typeName.ToLowerInvariant() switch
        {
            "int" => ValueType.Int,
            "float" => ValueType.Float,
            "string" => ValueType.String,
            "bool" => ValueType.Bool,
            "void" => ValueType.Void,
            _ => throw new TypeErrorException($"Unknown type name: {typeName}")
        };
    }

    private static ValueType? GetBinaryOperationResultType(BinaryOperation operation, ValueType left, ValueType right)
    {
        switch (operation)
        {
            case BinaryOperation.Add:
                if (left == ValueType.String && right == ValueType.String) return ValueType.String;
                if (left == right && (left == ValueType.Int || left == ValueType.Float)) return left;
                return null;
            case BinaryOperation.Subtract:
            case BinaryOperation.Multiply:
            case BinaryOperation.Divide:
                if (left == right && (left == ValueType.Int || left == ValueType.Float)) return left;
                return null;
            case BinaryOperation.Equal:
            case BinaryOperation.NotEqual:
                if (left == right && left != ValueType.Void) return ValueType.Bool;
                return null;
            case BinaryOperation.LessThan:
            case BinaryOperation.GreaterThan:
            case BinaryOperation.LessThanOrEqual:
            case BinaryOperation.GreaterThanOrEqual:
                if (left == right && left != ValueType.Void && left != ValueType.Bool) return ValueType.Bool;
                return null;
            case BinaryOperation.And:
            case BinaryOperation.Or:
                if (left == ValueType.Bool && right == ValueType.Bool) return ValueType.Bool;
                return null;
            default:
                return null;
        }
    }
}