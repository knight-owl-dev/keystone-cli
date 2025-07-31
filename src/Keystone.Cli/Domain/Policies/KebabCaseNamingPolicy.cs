using System.Text.RegularExpressions;


namespace Keystone.Cli.Domain.Policies;

/// <summary>
/// The Kebab-Case naming policy.
/// </summary>
public static partial class KebabCaseNamingPolicy
{
    /// <summary>
    /// Gets the project directory name based on the provided name.
    /// </summary>
    /// <param name="name">The name to convert.</param>
    /// <returns>
    /// A kebab-case version of the provided name, with all delimiters replaced by hyphens.
    /// </returns>
    public static string ToKebabCase(string name)
        => DelimitersRegex().Replace(name.Trim().ToLowerInvariant(), "-");

    /// <summary>
    /// The regular expression used to match delimiters in the name.
    /// </summary>
    /// <returns>
    /// The compiled regular expression that matches common delimiters such as spaces, commas,
    /// semicolons, plus signs, underscores, and hyphens.
    /// </returns>
    [GeneratedRegex(@"[\.,;+\s_-]+")]
    private static partial Regex DelimitersRegex();
}
