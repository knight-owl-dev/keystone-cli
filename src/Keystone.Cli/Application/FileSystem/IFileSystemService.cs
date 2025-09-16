namespace Keystone.Cli.Application.FileSystem;

/// <summary>
/// Defines a service for performing basic file system operations.
/// </summary>
/// <remarks>
/// This interface wraps around the <see cref="Directory"/> class to provide
/// a testable contract for file system operations.
/// </remarks>
public interface IFileSystemService
{
    /// <summary>
    /// Determines whether the specified directory exists in the file system.
    /// </summary>
    /// <param name="path">The path of the directory to check.</param>
    /// <returns>
    /// <c>true</c> if path refers to an existing directory; <c>false</c> if the directory does not exist
    /// or an error occurs when trying to determine if the specified directory exists.
    /// </returns>
    /// <seealso cref="Directory.Exists"/>
    bool DirectoryExists(string? path);

    /// <summary>
    /// Creates all directories and subdirectories in the specified path unless they already exist.
    /// </summary>
    /// <param name="path">The directory to create.</param>
    /// <returns>An object that represents the directory at the specified path. This object is returned regardless of whether a directory at the specified path already exists.</returns>
    /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The specified path exceeds the system-defined maximum length.</exception>
    /// <exception cref="IOException"><paramref name="path" /> is a file.</exception>
    /// <exception cref="DirectoryNotFoundException">A component of the <paramref name="path" /> is not a directory.</exception>
    /// <seealso cref="Directory.CreateDirectory(string)"/>
    DirectoryInfo CreateDirectory(string path);

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file to check.</param>
    /// <returns>
    /// <c>true</c> if the caller has the required permissions and path contains the name of an existing file;
    /// otherwise, <c>false</c>. This method also returns <c>false</c> if path is <c>null</c>, an invalid path,
    /// or a zero-length string. If the caller does not have sufficient permissions to read the specified file,
    /// no exception is thrown and the method returns false regardless of the existence of path.
    /// </returns>
    bool FileExists(string? path);

    /// <summary>
    /// Opens a read-only stream to the specified file with shared read access.
    /// </summary>
    /// <param name="path">The path of the file to open.</param>
    /// <returns>
    /// A stream for reading the file content.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="FileNotFoundException">The specified file was not found.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <seealso cref="FileStream"/>
    Stream OpenReadStream(string path);

    /// <summary>
    /// Opens a write stream to the specified file. If the file does not exist, it is created;
    /// otherwise, the existing file is opened without truncating or overwriting its contents.
    /// </summary>
    /// <param name="path">The file to create or overwrite.</param>
    /// <returns>
    /// A stream for writing to the file, positioned at the beginning.
    /// Existing contents will be preserved unless overwritten by the caller.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid.</exception>
    Stream OpenWriteStream(string path);
}
