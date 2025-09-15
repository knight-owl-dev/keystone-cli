namespace Keystone.Cli.Application.Utility.Serialization;

/// <summary>
/// Provides functionality for reading and writing plain text files, with support for ignoring comment lines.
/// </summary>
public interface ITextFileSerializer
{
    /// <summary>
    /// Loads all uncommented lines from the specified text file. Lines beginning with comment markers (e.g., <c>#</c>)
    /// are ignored and not included in the returned collection.
    /// </summary>
    /// <param name="path">The path to the file to read.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A read-only list of uncommented lines in the file.</returns>
    Task<IReadOnlyList<string>> LoadLinesAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the provided lines to the specified text file. Existing comment lines (e.g., <c>#</c>) are preserved,
    /// and new lines are appended to the end of the file.
    /// </summary>
    /// <param name="path">The path to the file to write.</param>
    /// <param name="lines">The lines to write to the file.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken = default);
}
