using System.Security.Cryptography;
using System.Text;


namespace Keystone.Cli.Application.Utility;

/// <summary>
/// The default implementation of <see cref="IContentHashService"/> for computing content hashes.
/// </summary>
public class ContentHashService
    : IContentHashService
{
    /// <inheritdoc />
    public string ComputeFromKeyValues(IDictionary<string, string?> content)
        => ComputeFromLines(
            content
                .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
                .Select(kvp => $"{kvp.Key}={kvp.Value ?? string.Empty}")
        );

    /// <inheritdoc />
    public string ComputeFromLines(IEnumerable<string> content)
        => content.Aggregate(
            new StringBuilder(),
            (acc, line) => acc.AppendLine(line),
            acc => ComputeFromString(acc.ToString())
        );

    /// <inheritdoc />
    public string ComputeFromString(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}
