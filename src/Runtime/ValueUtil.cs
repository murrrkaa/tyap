using System.Text;

namespace PsTiger.Runtime;

/// <summary>
/// Вспомогательные методы для форматирования значений.
/// Поддерживает только базовые типы: string.
/// </summary>
internal static class ValueUtil
{
    /// <summary>
    /// Форматирует строковое значение в кавычках с экранированием.
    /// Поддерживает: \\ и \'
    /// </summary>
    internal static string EscapeStringValue(string s)
    {
        StringBuilder sb = new StringBuilder();  // ← явный тип
        sb.Append('\'');

        foreach (char c in s)  // ← явный тип в foreach
        {
            if (c == '\\') sb.Append(@"\\");
            else if (c == '\'') sb.Append(@"\'");
            else sb.Append(c);
        }

        sb.Append('\'');
        return sb.ToString();
    }
}