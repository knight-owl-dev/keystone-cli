namespace Keystone.Cli.Application.Utility.Serialization;

/// <summary>
/// Provides functionality for reading and writing JSON files using strongly typed models.
/// </summary>
public interface IJsonFileSerializer
{
    /// <summary>
    /// Loads and deserializes JSON content from the specified file path into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON content into.</typeparam>
    /// <param name="path">The full path to the JSON file to read.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous load operation, containing the deserialized instance of <typeparamref name="T"/>.
    /// </returns>
    /// <exception cref="DirectoryNotFoundException">
    /// The specified path is invalid, such as being on an unmapped drive.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown if the specified file does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if deserialization fails or the JSON content is invalid.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the application lacks necessary permissions to access the file.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled via the provided cancellation token.
    /// </exception>
    Task<T> LoadAsync<T>(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializes the specified data as JSON and writes it to the given file path.
    /// </summary>
    /// <remarks>
    /// JSON comments are not preserved when saving the file.
    /// </remarks>
    /// <typeparam name="T">The type of the data being serialized.</typeparam>
    /// <param name="path">The full path to the JSON file to write.</param>
    /// <param name="data">The data to serialize.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
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
    Task SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default);
}
