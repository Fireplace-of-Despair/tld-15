using System.Text;

namespace TLD15.Helpers;

public static class StringHelper
{
    public static string TrimLast(this string text, char c)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var lastSlash = text.LastIndexOf(c);
        return (lastSlash > -1) ? text[..lastSlash] : text;
    }

    public static string TrimByWord(this string text, int maxLength)
    {
        text = text.Trim();

        if (text.Length <= maxLength)
        {
            return text;
        }

        var truncatedText = new StringBuilder();
        var currentLength = 0;
        var lastSpaceIndex = -1;

        for (var i = 0; i < text.Length; i++)
        {
            var currentChar = text[i];
            if (currentChar == ' ')
            {
                lastSpaceIndex = i;
            }

            truncatedText.Append(currentChar);
            currentLength++;

            if (currentLength >= maxLength)
            {
                break;
            }
        }

        // If truncation is happening mid-word, adjust the output
        if (currentLength == maxLength && lastSpaceIndex > -1)
        {
            truncatedText.Length = lastSpaceIndex;
        }

        truncatedText.Append("...");

        return truncatedText.ToString();
    }

}
