using PsTiger.Ast.Statements;  // ← Обязательно добавьте это!

namespace PsTiger.Ast.Declarations;

/// <summary>
/// Объявление пользовательской функции: function name(params): returnType { body }.
/// </summary>
public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
    public FunctionDeclaration(
        string name,
        IReadOnlyList<ParameterDeclaration> parameters,
        string returnType,
        BlockStatement body
    ) : base(name, parameters.Cast<AbstractParameterDeclaration>().ToList().AsReadOnly())
    {
        Name = name;
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
    }

    public new string Name { get; }

    public new IReadOnlyList<ParameterDeclaration> Parameters { get; }

    public string ReturnType { get; }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}