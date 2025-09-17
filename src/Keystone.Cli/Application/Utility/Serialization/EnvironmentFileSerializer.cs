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
    public Task SaveAsync(string path, IDictionary<string, string?> values, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
