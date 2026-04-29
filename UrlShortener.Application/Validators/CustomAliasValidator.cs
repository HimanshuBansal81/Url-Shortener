using System.Text.RegularExpressions;

namespace UrlShortener.Application.Validators;

public static partial class CustomAliasValidator
{
    [GeneratedRegex("^[a-zA-Z0-9-]{3,30}$")]
    private static partial Regex AliasRegex();

    public static string? Validate(string? customAlias)
    {
        if (string.IsNullOrWhiteSpace(customAlias))
        {
            return null;
        }

        var alias = customAlias.Trim();

        if (!AliasRegex().IsMatch(alias))
        {
            throw new ArgumentException("Custom alias must be 3-30 characters and contain only letters, numbers, or dashes.", nameof(customAlias));
        }

        return alias;
    }
}
