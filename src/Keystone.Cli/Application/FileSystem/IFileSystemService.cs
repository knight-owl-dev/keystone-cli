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
    /// Opens a <see cref="Stream"/> on the specified path, having the specified mode with read, write, or read/write
    /// access and the specified sharing option.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <param name="mode">
    /// A <see cref="FileMode"/> value that specifies whether a file is created if one does not exist,
    /// and determines whether the contents of existing files are retained or overwritten.
    /// </param>
    /// <param name="access">
    /// A <see cref="FileAccess"/> value that specifies the operations that can be performed on the file.
    /// </param>
    /// <param name="share">
    /// A <see cref="FileShare"/> value specifying the type of access other threads have to the file.
    /// </param>
    /// <returns>
    /// A <see cref="Stream"/> on the specified path, having the specified mode with read, write, or read/write access
    /// and the specified sharing option.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="mode"/> is incompatible with the file access specified by <paramref name="access"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="path"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="PathTooLongException">
    /// Thrown when the specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown when the specified path is invalid, such as being on an unmapped drive.
    /// </exception>
    /// <exception cref="IOException">
    /// Thrown when an I/O error occurs, such as the file being in use by another process or the disk being full.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the caller does not have the required permission to access the file,
    /// or the file is read-only and <paramref name="access"/> specifies write access.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown when the file specified by <paramref name="path"/> does not exist and <paramref name="mode"/> is set to <see cref="FileMode.Open"/>
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when the <paramref name="path"/> is in an invalid format, such as containing invalid characters.
    /// </exception>
    /// <seealso cref="File.Open(string, FileMode, FileAccess, FileShare)"/>
    Stream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);
}
