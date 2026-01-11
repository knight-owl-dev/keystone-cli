using System.Collections.Immutable;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility.Serialization.Yaml;
using Keystone.Cli.Application.Utility.Text;


namespace Keystone.Cli.Application.Utility.Serialization;

/// <summary>
/// The default implementation of <see cref="IYamlFileSerializer"/>.
/// </summary>
public class YamlFileSerializer(IFileSystemService fileSystemService)
    : IYamlFileSerializer
{
    /// <inheritdoc />
    public async Task<IDictionary<string, YamlValue>> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenReadStream(path);

        var unfilteredLines = await TextFileUtility.ReadLinesAsync(stream, cancellationToken).ConfigureAwait(false);
        try
        {
            return YamlParsingUtility.Parse(unfilteredLines)
                .Where(entry => entry is { PropertyName: not null, Kind: not YamlParsingUtility.EntryKind.Unknown })
                .ToImmutableDictionary(entry => entry.PropertyName!, YamlSerializationHelpers.GetValue);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException($"Could not parse environment file '{path}': {exception.Message}", exception);
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(string path, IDictionary<string, YamlValue> data, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenWriteStream(path);

        var unfilteredLines = await TextFileUtility.ReadLinesAsync(stream, cancellationToken).ConfigureAwait(false);
        var currentEntries = YamlParsingUtility.Parse(unfilteredLines);
        stream.Seek(0, SeekOrigin.Begin);

        var composedEntries = ComposeLines(currentEntries.ToImmutableList(), data);
        var composedLines = composedEntries.SelectMany(entry => entry.RawLines);

        await TextFileUtility.WriteLinesAsync(stream, composedLines, cancellationToken).ConfigureAwait(false);
        stream.SetLength(stream.Position);
    }

    /// <summary>
    /// Composes a new set of YAML entries for a YAML file by preserving all leading and trailing non-content lines
    /// (comments or whitespace) from the current file, and replacing the content lines in between with the provided
    /// YAML values.
    /// </summary>
    /// <remarks>
    /// If the original property-value pair is the same as the new one, it is preserved in its original form. This includes
    /// preserving the order of existing properties and only adding new properties at the end of the content section.
    /// </remarks>
    /// <param name="currentEntries">A collection of all current entries including comments and whitespace.</param>
    /// <param name="values">A collection of values.</param>
    /// <returns>
    /// A collection containing the header (leading non-content lines), the new key-value pairs,
    /// </returns>
    private static ImmutableList<YamlParsingUtility.Entry> ComposeLines(
        ImmutableList<YamlParsingUtility.Entry> currentEntries,
        IDictionary<string, YamlValue> values
    )
        => currentEntries.Aggregate(
            (
                Entries: ImmutableList.CreateBuilder<YamlParsingUtility.Entry>(),
                ExistingKeys: ImmutableHashSet.CreateBuilder<string>(),
                Values: values
            ),
            (acc, currentEntry) =>
            {
                if (currentEntry is { PropertyName: not null })
                {
                    var key = currentEntry.PropertyName!;
                    var newEntry = acc.Values.TryGetValue(key, out var newValue) && ! YamlSerializationHelpers.IsEqualValue(currentEntry, newValue)
                        ? YamlSerializationHelpers.ToEntry(key, newValue)
                        : currentEntry;

                    acc.ExistingKeys.Add(key);
                    acc.Entries.Add(newEntry);
                }
                else
                {
                    acc.Entries.Add(currentEntry);
                }

                return acc;
            },
            acc =>
            {
                var existingKeys = acc.ExistingKeys.ToImmutable();

                var newEntries = acc.Values
                    .Where(kvp => ! existingKeys.Contains(kvp.Key))
                    .Select(YamlSerializationHelpers.AsEntry);

                // insert new lines before the terminator entry, if any
                var terminatorIndex = acc.Entries.FindIndex(YamlParsingUtility.IsTerminatorEntry);
                if (terminatorIndex >= 0)
                {
                    acc.Entries.InsertRange(terminatorIndex, newEntries);
                }
                else
                {
                    acc.Entries.AddRange(newEntries);
                }

                return acc.Entries.ToImmutable();
            }
        );
}
