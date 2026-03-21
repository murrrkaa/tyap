namespace PsTiger.Ast.Declarations;

/// <summary>
/// Абстрактный класс с информацией о переменной или формальном параметре функции.
/// </summary>
public abstract class AbstractVariableDeclaration : Declaration
{
    protected AbstractVariableDeclaration(
        string name, 
        string typeName
        )
    {
        Name = name;
        TypeName = typeName;

    }

    public string Name { get; }
    public string TypeName { get; } // int, float, string, bool
}