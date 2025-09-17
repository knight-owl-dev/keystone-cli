namespace Keystone.Cli.Application.Utility.Text;

/// <summary>
/// Common text parsing utilities.
/// </summary>
public static class TextParsingUtility
{
    /// <summary>
    /// Checks if a line is either empty, contains only whitespace, or is a comment line (starts with '#').
    /// </summary>
    /// <param name="line">The content line.</param>
    /// <returns>
    /// <c>true</c> if the line is empty, whitespace, or a comment; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWhiteSpaceOrCommentLine(string line)
        => string.IsNullOrWhiteSpace(line) || line.StartsWith('#');

    /// <summary>
    /// Formats a key-value pair as a string in the form <c>KEY=VALUE</c>.
    /// </summary>
    /// <param name="kvp">The key-value pair to format.</param>
    /// <returns>A string in the form <c>KEY=VALUE</c>. If the value is <c>null</c>, an empty string is used for the value.</returns>
    public static string GetKeyValueString(KeyValuePair<string, string?> kvp)
        => GetKeyValueString(kvp.Key, kvp.Value);

    /// <summary>
    /// Formats a key and value as a string in the form <c>KEY=VALUE</c>.
    /// </summary>
    /// <param name="key">The key to format.</param>
    /// <param name="value">The value to format. If <c>null</c>, an empty string is used for the value.</param>
    /// <returns>A string in the form <c>KEY=VALUE</c>.</returns>
    public static string GetKeyValueString(string key, string? value)
        => $"{key}={value ?? string.Empty}";

    /// <summary>
    /// Parses a key-value pair from a content line.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Lines must be in the format <c>KEY=VALUE</c>. Lines that do not conform to this format are ignored.
    /// </para>
    /// <para>
    /// This method does not handle comments or blank lines; use <see cref="IsWhiteSpaceOrCommentLine"/>
    /// to filter those out first.
    /// </para>
    /// </remarks>
    /// <param name="line">The content line.</param>
    /// <returns>
    /// A parsed key-value pair, or an empty record if the line is not a valid key-value pair.
    /// </returns>
    public static KeyValuePair<string, string?> ParseKeyValuePair(string line)
    {
        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            return default;
        }

        return Normalize(line[..separatorIndex]) is { } key
            ? new KeyValuePair<string, string?>(key, value: Normalize(line[(separatorIndex + 1)..]))
            : default;
    }

    /// <summary>
    /// Normalizes a text value by trimming leading and trailing whitespace.
    /// </summary>
    /// <param name="text">A text value.</param>
    /// <returns>
    /// A trimmed text value, or <c>null</c> if the input is <c>null</c>, empty, or consists only of whitespace.
    /// </returns>
    public static string? Normalize(string text)
        => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
