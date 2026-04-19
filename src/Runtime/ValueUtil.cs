using System.Text;

namespace Mlt.Runtime;

internal static class ValueUtil
{
    internal static string EscapeStringValue(string s)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('\'');

        foreach (char c in s)
        {
            if (c == '\\')
            {
                sb.Append(@"\\");
            }
            else if (c == '\'')
            {
                sb.Append(@"\'");
            }
            else
            {
                sb.Append(c);
            }
        }

        sb.Append('\'');
        return sb.ToString();
    }
}