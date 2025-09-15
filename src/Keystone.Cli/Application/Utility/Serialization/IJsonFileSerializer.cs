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
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default);
}
