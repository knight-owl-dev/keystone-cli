namespace Keystone.Cli.Application.Utility;

/// <summary>
/// Provides hashing functionality for content comparisons and change tracking.
/// </summary>
public interface IContentHashService
{
    /// <summary>
    /// Computes an SHA-256 hash from the given key-value pairs.
    /// </summary>
    /// <remarks>
    /// Keys are ordered deterministically before hashing.
    /// </remarks>
    /// <param name="content">The content represented as key-value pairs.</param>
    /// <returns>
    /// A hexadecimal string representing the hash.
    /// </returns>
    string ComputeFromKeyValues(IDictionary<string, string?> content);

    /// <summary>
    /// Computes an SHA-256 hash from the given lines of text.
    /// </summary>
    /// <param name="content">The content represented as individual lines of text.</param>
    /// <returns>
    /// A hexadecimal string representing the hash.
    /// </returns>
    string ComputeFromLines(IEnumerable<string> content);

    /// <summary>
    /// Computes an SHA-256 hash from the given string content.
    /// </summary>
    /// <param name="content">The string content.</param>
    /// <returns>
    /// A hexadecimal string representing the hash.
    /// </returns>
    string ComputeFromString(string content);
}
