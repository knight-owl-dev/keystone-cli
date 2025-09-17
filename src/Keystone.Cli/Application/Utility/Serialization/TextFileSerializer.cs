using System.Collections.Immutable;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility.Text;


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

        var unfilteredLines = await TextFileUtility.ReadLinesAsync(stream, cancellationToken).ConfigureAwait(false);
        var contentLines = unfilteredLines.RemoveAll(TextParsingUtility.IsWhiteSpaceOrCommentLine);

        return contentLines;
    }

    /// <inheritdoc />
    public async Task SaveLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenWriteStream(path);

        var currentLines = await TextFileUtility.ReadLinesAsync(stream, cancellationToken).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);

        var composedLines = currentLines.IsEmpty
            ? lines
            : ComposeLines(currentLines, newContentLines: lines);

        await TextFileUtility.WriteLinesAsync(stream, composedLines, cancellationToken).ConfigureAwait(false);
        stream.SetLength(stream.Position);
    }

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
        composedLines.AddRange(currentLines.TakeWhile(TextParsingUtility.IsWhiteSpaceOrCommentLine));

        var trailerStart = composedLines.Count == currentLines.Count
            ? currentLines.Count
            : currentLines.Count - currentLines.Reverse().TakeWhile(TextParsingUtility.IsWhiteSpaceOrCommentLine).Count();

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
