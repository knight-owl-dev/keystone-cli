using System.Text.Json;
using Keystone.Cli.Application.FileSystem;


namespace Keystone.Cli.Application.Utility.Serialization;

/// <summary>
/// The default implementation of <see cref="IJsonFileSerializer"/> that provides JSON file serialization
/// and deserialization using the default <see cref="JsonSerializer"/>.
/// </summary>
public class JsonFileSerializer(IFileSystemService fileSystemService)
    : IJsonFileSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
    };

    /// <inheritdoc />
    public async Task<T> LoadAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenReadStream(path);

        try
        {
            var result = await JsonSerializer
                .DeserializeAsync<T>(stream, DefaultOptions, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result ?? throw new InvalidOperationException($"Failed to deserialize JSON content from: {path}.");
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException($"Failed to deserialize JSON content from: {path}.", exception);
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync<T>(string path, T data, CancellationToken cancellationToken = default)
    {
        await using var stream = fileSystemService.OpenWriteStream(path);

        await JsonSerializer
            .SerializeAsync(stream, data, DefaultOptions, cancellationToken)
            .ConfigureAwait(false);
    }
}
