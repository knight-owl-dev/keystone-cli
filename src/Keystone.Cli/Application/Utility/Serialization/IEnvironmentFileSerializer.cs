namespace Keystone.Cli.Application.Utility.Serialization;

/// <summary>
/// Defines methods for loading and saving key-value pairs from an environment (<c>.env</c>) file.
/// </summary>
public interface IEnvironmentFileSerializer
{
    /// <summary>
    /// Loads key-value pairs from a <c>.env</c> file.
    /// </summary>
    /// <param name="path">The full path to the <c>.env</c> file.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous load operation.
    /// The task result contains a dictionary of environment variable keys and values.
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
    /// <exception cref="InvalidOperationException">
    /// Thrown if the file contains duplicate keys or cannot be parsed correctly.
    /// </exception>
    Task<IDictionary<string, string?>> LoadAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the specified key-value pairs to a <c>.env</c> file, preserving existing formatting and comments.
    /// </summary>
    /// <remarks>
    /// This operation preserves all original comments (lines starting with <c>#</c>) and blank lines.
    /// If a key already exists in the file, its value is updated in place; otherwise, a new entry is added.
    /// </remarks>
    /// <param name="path">The full path to the <c>.env</c> file.</param>
    /// <param name="values">The key-value pairs to save to the file.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
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
    Task SaveAsync(string path, IDictionary<string, string?> values, CancellationToken cancellationToken = default);
}
