﻿using System.Text;

namespace Common.Utils;

public static class StringHelper
{
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

        // If the truncated string is exactly the maxLength, remove the last word and append "..."
        if (currentLength == maxLength && lastSpaceIndex > -1)
        {
            truncatedText.Remove(lastSpaceIndex, currentLength - lastSpaceIndex);
        }

        truncatedText.Append("...");

        return truncatedText.ToString();
    }
}
