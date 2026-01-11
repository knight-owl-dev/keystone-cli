using System.Collections.Immutable;


namespace Keystone.Cli.Application.Utility.Text;

/// <summary>
/// Utility methods for working with text files.
/// </summary>
public static class TextFileUtility
{
    /// <summary>
    /// Reads all lines from the given stream asynchronously. Keeps the stream open after reading.
    /// </summary>
    /// <param name="stream">The input stream properly positioned.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An immutable list of unfiltered lines read from the stream. Otherwise, an empty list if the stream is empty.
    /// </returns>
    public static async Task<ImmutableList<string>> ReadLinesAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);

        var lines = ImmutableList.CreateBuilder<string>();
        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            lines.Add(line);
        }

        return lines.ToImmutable();
    }

    /// <summary>
    /// Writes the given lines to the provided stream asynchronously. Keeps the stream open after writing.
    /// </summary>
    /// <param name="stream">The target stream properly positioned.</param>
    /// <param name="lines">New lines.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task WriteLinesAsync(Stream stream, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(stream, leaveOpen: true);

        foreach (var line in lines)
        {
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
        }
    }
}
