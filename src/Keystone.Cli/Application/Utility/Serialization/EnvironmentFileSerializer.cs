using System.Collections.Immutable;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility.Text;


namespace Keystone.Cli.Application.Utility.Serialization;

/// <summary>
/// The default implementation of <see cref="IEnvironmentFileSerializer"/>.
/// </summary>
public class EnvironmentFileSerializer(IFileSystemService fileSystemService)
    : IEnvironmentFileSerializer
{
    /// <inheritdoc />
    public async Task<IDictionary<string, string?>> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenReadStream(path);

        var unfilteredLines = await TextFileUtility.ReadLinesAsync(stream, cancellationToken).ConfigureAwait(false);
        var contentLines = unfilteredLines.RemoveAll(TextParsingUtility.IsWhiteSpaceOrCommentLine);

        return contentLines.Select(TextParsingUtility.ParseKeyValuePair)
            .Where(kvp => kvp is { Key: not null })
            .ToImmutableDictionary();
    }

    /// <inheritdoc />
    public async Task SaveAsync(string path, IDictionary<string, string?> values, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenWriteStream(path);

        var currentLines = await TextFileUtility.ReadLinesAsync(stream, cancellationToken).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);

        var composedLines = ComposeLines(currentLines, values);

        await TextFileUtility.WriteLinesAsync(stream, composedLines, cancellationToken).ConfigureAwait(false);
        stream.SetLength(stream.Position);
    }

    /// <summary>
    /// Composes a new set of lines for an environment file by preserving all leading and trailing non-content lines
    /// (comments or whitespace) from the current file, and replacing the content lines in between with
    /// the provided key-value pairs formatted as "KEY=VALUE".
    /// </summary>
    /// <remarks>
    /// If the original key-value pair is the same as the new one, it should be preserved in its original form.
    /// This includes preserving the order of existing keys and only adding new keys at the end of the content section.
    /// </remarks>
    /// <param name="currentLines">A collection of all current lines including comments and whitespace.</param>
    /// <param name="values">A collection of values.</param>
    /// <returns>
    /// A collection containing the header (leading non-content lines), the new key-value pairs,
    /// </returns>
    private static ImmutableList<string> ComposeLines(ImmutableList<string> currentLines, IDictionary<string, string?> values)
        => currentLines.Select(line =>
            new
            {
                Line = line,
                KeyValuePair = TextParsingUtility.ParseKeyValuePair(line),
            }
        ).Aggregate(
            new
            {
                Lines = ImmutableList.CreateBuilder<string>(),
                ExistingKeys = ImmutableHashSet.CreateBuilder<string>(),
                Values = values,
            },
            (acc, item) =>
            {
                if (item.KeyValuePair is { Key: null })
                {
                    acc.Lines.Add(item.Line);

                    return acc;
                }

                var (key, currentValue) = item.KeyValuePair;
                acc.ExistingKeys.Add(key);

                if (acc.Values.TryGetValue(key, out var newValue) && newValue != currentValue)
                {
                    acc.Lines.Add(TextParsingUtility.GetKeyValueString(key, newValue));
                }
                else
                {
                    acc.Lines.Add(item.Line);
                }

                return acc;
            },
            acc =>
            {
                var existingKeys = acc.ExistingKeys.ToImmutable();

                var newLines = acc.Values
                    .Where(kvp => ! existingKeys.Contains(kvp.Key))
                    .Select(TextParsingUtility.GetKeyValueString);

                acc.Lines.AddRange(newLines);

                return acc.Lines.ToImmutable();
            }
        );
}
