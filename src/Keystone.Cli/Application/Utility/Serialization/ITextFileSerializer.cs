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
    /// <returns>
    /// A read-only list of uncommented lines in the file.
    /// </returns>
    /// <exception cref="DirectoryNotFoundException">
    /// The specified path is invalid, such as being on an unmapped drive.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown if the specified file does not exist.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the application lacks necessary permissions to access the file.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled via the provided cancellation token.
    /// </exception>
    Task<IReadOnlyList<string>> LoadLinesAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the provided lines to the specified text file. Existing comment lines (e.g., <c>#</c>) are preserved,
    /// and new lines are appended to the end of the file.
    /// </summary>
    /// <param name="path">The path to the file to write.</param>
    /// <param name="lines">The lines to write to the file.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the application lacks necessary permissions to write to the file.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if the specified directory does not exist.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled via the provided cancellation token.
    /// </exception>
    Task SaveLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken = default);
}
