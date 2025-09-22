using Keystone.Cli.Application.Utility.Serialization.Yaml;


namespace Keystone.Cli.Application.Utility.Serialization;

/// <summary>
/// Provides functionality for reading and writing YAML files using strongly typed models.
/// </summary>
public interface IYamlFileSerializer
{
    /// <summary>
    /// Loads and deserializes YAML content from the specified file path into a dictionary.
    /// </summary>
    /// <param name="path">The full path to the YAML file to read.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous load operation, containing the deserialized data.
    /// </returns>
    /// <exception cref="DirectoryNotFoundException">
    /// The specified path is invalid, such as being on an unmapped drive.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown if the specified file does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if deserialization fails or the YAML content is invalid.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the application lacks necessary permissions to access the file.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled via the provided cancellation token.
    /// </exception>
    Task<IDictionary<string, YamlValue>> LoadAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializes the specified data as YAML and writes it to the given file path.
    /// </summary>
    /// <remarks>
    /// YAML comments are preserved when saving the file.
    /// </remarks>
    /// <param name="path">The full path to the YAML file to write.</param>
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
    Task SaveAsync(string path, IDictionary<string, YamlValue> data, CancellationToken cancellationToken = default);
}
