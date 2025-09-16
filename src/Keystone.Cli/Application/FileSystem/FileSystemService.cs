namespace Keystone.Cli.Application.FileSystem;

/// <summary>
/// The file system service that provides basic directory operations.
/// </summary>
public class FileSystemService
    : IFileSystemService
{
    /// <inheritdoc />
    public bool DirectoryExists(string? path)
        => Directory.Exists(path);

    /// <inheritdoc />
    public DirectoryInfo CreateDirectory(string path)
        => Directory.CreateDirectory(path);

    /// <inheritdoc />
    public bool FileExists(string? path)
        => File.Exists(path);

    /// <inheritdoc />
    public Stream OpenReadStream(string path)
        => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    /// <inheritdoc />
    public Stream OpenWriteStream(string path)
        => new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
}
