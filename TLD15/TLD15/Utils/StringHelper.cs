namespace TLD15.Utils;

public static class StringHelper
{
    public static string TrimLast(this string text, char c)
    {
        int lastSlash = text.LastIndexOf(c);
        return (lastSlash > -1) ? text.Substring(0, lastSlash) : text;
    }
}
