using System.Text.RegularExpressions;

namespace Scaffolding.Utilities;

internal static class StringUtility
{
    public static string ToCase(this string value, string strategy)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        strategy = strategy?.ToLowerInvariant().Trim();
        switch (strategy)
        {
            case "snake":
            case "snakecase":
                return value.ToSnakeCase();
            case "camel":
            case "camelcase":
                return value.ToCamelCase();
            case "lower":
            case "lowercase":
                return value.ToLowerCase();
            default:
                return value;
        }
    }

    public static string ToSnakeCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        text = text.ToCamelCase();
        text = string.Concat(text.Select((char _char, int i) => (i <= 0 || !char.IsUpper(_char)) ? _char.ToString() : ("_" + _char))).ToLower();
        return text;
    }

    public static string ToCamelCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        string text2 = "";
        bool flag = false;
        for (int i = 0; i < text.Length - 1; i++)
        {
            text2 += (flag ? char.ToUpperInvariant(text[i]) : text[i]);
            flag = text[i] == '_';
        }

        text2 += text[text.Length - 1];
        text2 = text2.Replace("_", "");
        if (text2.Length == 0)
        {
            return null;
        }

        text2 = Regex.Replace(text2, "([A-Z])([A-Z]+)($|[A-Z])", (Match m) => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
        return char.ToLowerInvariant(text2[0]) + text2.Substring(1);
    }

    public static string ToLowerCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return text.ToLowerInvariant().Replace("_", "");
    }

}
