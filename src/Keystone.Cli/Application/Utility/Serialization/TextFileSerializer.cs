using System.Collections.Immutable;
using Keystone.Cli.Application.FileSystem;


namespace Keystone.Cli.Application.Utility.Serialization;

/// <summary>
/// The default implementation of <see cref="ITextFileSerializer"/> that reads and writes plain text files.
/// </summary>
public class TextFileSerializer(IFileSystemService fileSystemService)
    : ITextFileSerializer
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> LoadLinesAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenReadStream(path);

        var unfilteredLines = await ReadLinesAsync(stream, cancellationToken).ConfigureAwait(false);
        var contentLines = unfilteredLines.RemoveAll(IsWhiteSpaceOrCommentLine);

        return contentLines;
    }

    /// <inheritdoc />
    public async Task SaveLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenWriteStream(path);

        var currentLines = await ReadLinesAsync(stream, cancellationToken).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);

        var composedLines = currentLines.IsEmpty
            ? lines
            : ComposeLines(currentLines, newContentLines: lines);

        await WriteLinesAsync(stream, composedLines, cancellationToken).ConfigureAwait(false);
        stream.SetLength(stream.Position);
    }

    /// <summary>
    /// Reads all lines from the given stream asynchronously. Keeps the stream open after reading.
    /// </summary>
    /// <param name="stream">The input stream properly positioned.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An immutable list of unfiltered lines read from the stream. Otherwise, an empty list if the stream is empty.
    /// </returns>
    private static async Task<ImmutableList<string>> ReadLinesAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);

        var lines = ImmutableList.CreateBuilder<string>();
        while (! reader.EndOfStream)
        {
            if (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
            {
                lines.Add(line);
            }
        }

        return lines.ToImmutable();
    }

    /// <summary>
    /// Writes the given lines to the provided stream asynchronously. Keeps the stream open after writing.
    /// </summary>
    /// <param name="stream">The target stream properly positioned.</param>
    /// <param name="lines">New lines.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private static async Task WriteLinesAsync(Stream stream, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(stream, leaveOpen: true);

        foreach (var line in lines)
        {
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Checks if a line is either empty, contains only whitespace, or is a comment line (starts with '#').
    /// </summary>
    /// <param name="line">The input line.</param>
    /// <returns>
    /// <c>true</c> if the line is empty, whitespace, or a comment; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsWhiteSpaceOrCommentLine(string line)
        => string.IsNullOrWhiteSpace(line) || line.StartsWith('#');

    /// <summary>
    /// Composes a new set of lines for a text file by preserving all leading and trailing non-content lines
    /// (comments or whitespace) from the current file, and replacing the content lines in between with
    /// the provided new content lines.
    /// </summary>
    /// <param name="currentLines">The current lines from the file, including comments and whitespace.</param>
    /// <param name="newContentLines">The new content lines to insert, replacing the existing content lines.</param>
    /// <returns>
    /// A collection containing the header (leading non-content lines), the new content lines,
    /// and the footer (trailing non-content lines) from the original file.
    /// </returns>
    private static ImmutableList<string> ComposeLines(ImmutableList<string> currentLines, IEnumerable<string> newContentLines)
    {
        var composedLines = ImmutableList.CreateBuilder<string>();

        // add leading non-content lines (comments or whitespace)
        composedLines.AddRange(currentLines.TakeWhile(IsWhiteSpaceOrCommentLine));

        var trailerStart = composedLines.Count == currentLines.Count
            ? currentLines.Count
            : currentLines.Count - currentLines.Reverse().TakeWhile(IsWhiteSpaceOrCommentLine).Count();

        // add new content lines
        composedLines.AddRange(newContentLines);

        if (trailerStart < currentLines.Count)
        {
            // add trailing non-content lines (comments or whitespace)
            composedLines.AddRange(currentLines.Skip(trailerStart));
        }

        return composedLines.ToImmutableList();
    }
}
