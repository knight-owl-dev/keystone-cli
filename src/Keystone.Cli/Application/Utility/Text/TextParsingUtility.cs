using System.Text.RegularExpressions;


namespace Keystone.Cli.Application.Utility.Text;

/// <summary>
/// Common text parsing utilities.
/// </summary>
/// <remarks>
/// <para>
/// This utility is compatible with both regular text files and environment files.
/// It provides methods to identify comment and whitespace lines, format and parse
/// key-value pairs.
/// </para>
/// <para>
/// Environment key-value formatting is compatible with Docker-compose and similar tools.
/// For more information, refer to <a href="https://docs.docker.com/compose/env-file/">
/// Docker Compose Environment File documentation</a>.
/// </para>
/// </remarks>
public static partial class TextParsingUtility
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
    {
        if (string.IsNullOrEmpty(value))
        {
            return $"{key}=";
        }

        return GetQuoteStyle(value) switch
        {
            QuoteStyle.None => $"{key}={value}",
            QuoteStyle.Single => $"{key}='{value}'",
            QuoteStyle.Double => $"{key}=\"{EscapeDoubleQuotedString(value)}\"",
            _ => throw new InvalidOperationException("Unexpected quote style."),
        };
    }

    /// <summary>
    /// Tries to parse a key-value pair from a content line.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="ParseKeyValuePair"/>, this method returns <c>false</c> if the line is a comment or blank line.
    /// </remarks>
    /// <param name="line">The content line.</param>
    /// <param name="keyValuePair">The resulting key-value pair.</param>
    /// <returns>
    /// <c>true</c> if the line was successfully parsed into a key-value pair; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParseKeyValuePair(string line, out KeyValuePair<string, string?> keyValuePair)
    {
        if (IsWhiteSpaceOrCommentLine(line))
        {
            keyValuePair = default;

            return false;
        }

        keyValuePair = ParseKeyValuePair(line);

        return keyValuePair is { Key: not null };
    }

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
    private static string? Normalize(string text)
        => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

    /// <summary>
    /// Supported quoting styles for key-value pair values.
    /// </summary>
    private enum QuoteStyle
    {
        /// <summary>
        /// No quoting, no escaping necessary for simple values.
        /// </summary>
        None,

        /// <summary>
        /// Single-quoting with <c>'</c> and no escaping is necessary.
        /// </summary>
        Single,

        /// <summary>
        /// Double-quoting with <c>"</c> escaping is necessary.
        /// </summary>
        Double,
    }

    /// <summary>
    /// Gets the appropriate quoting style for a value when serializing a key-value pair.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If someone uses literal control sequences in their data, like <c>\r</c>, <c>\n</c>, <c>\t</c>, or <c>\\</c>,
    /// we need to single-quote the value to prevent them from being interpreted.
    /// </para>
    /// <para>
    /// Otherwise, if the value contains whitespace, <c>#</c>, or single quote (<c>'</c>) characters,
    /// we need to double-quote the value and escape special characters.
    /// </para>
    /// <para>
    /// Simple values that do not contain any of these characters can be left unquoted.
    /// </para>
    /// </remarks>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The appropriate quoting style for the value.
    /// </returns>
    private static QuoteStyle GetQuoteStyle(string value)
    {
        if (GetLiteralControlSequenceRx().IsMatch(value) || value.Contains('"'))
        {
            return QuoteStyle.Single;
        }

        return GetComplexSequenceRx().IsMatch(value)
            ? QuoteStyle.Double
            : QuoteStyle.None;
    }

    /// <summary>
    /// Escapes special characters in a double-quoted string value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The escaped string value.
    /// </returns>
    private static string EscapeDoubleQuotedString(string value)
        => value
            .Replace("\\", @"\\")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\"", "\\\"");

    /// <summary>
    /// Gets a regular expression to check for literal control sequences.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values containing literal control sequences like <c>\r</c>, <c>\n</c>, <c>\t</c>, or <c>\\</c>
    /// need to be single-quoted to prevent them from being interpreted.
    /// </para>
    ///
    /// <para>
    /// Note, this expression does not check for <c>"</c> since that is handled separately. Ideally,
    /// strings containing <c>"</c> should also be single-quoted, but this is not strictly necessary.
    /// </para>
    /// </remarks>
    /// <returns>
    /// The compiled regular expression.
    /// </returns>
    [GeneratedRegex(@"\\[rnt\\]")]
    private static partial Regex GetLiteralControlSequenceRx();

    /// <summary>
    /// Gets a regular expression to check for complex sequences.
    /// </summary>
    /// <remarks>
    /// Values need to be double-quoted if they contain whitespace, <c>#</c>, or single quote (<c>'</c>) characters.
    /// </remarks>
    /// <returns>
    /// The compiled regular expression.
    /// </returns>
    [GeneratedRegex(@"[\s#']")]
    private static partial Regex GetComplexSequenceRx();
}
