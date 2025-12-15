using System.Text.RegularExpressions;

namespace ZenithX;

public static class Utility
{
    public static string RemoveHtmlTags(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return Regex.Replace(input, "<[^>]+>", string.Empty);
    }

    public static bool IsNameValid(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        // Example validation: Name must be 1-10 characters, alphanumeric with some allowed special characters
        if (name.Length < 1 || name.Length > 10)
            return false;

        // Allow alphanumeric characters, spaces, and some common symbols
        return Regex.IsMatch(name, @"^[a-zA-Z0-9 _\-!?.]+$");
    }
}