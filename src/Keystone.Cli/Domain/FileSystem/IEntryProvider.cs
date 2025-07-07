namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// Defines an entry provider that supplies file system entries (files or directories) for reading or extraction.
/// </summary>
public interface IEntryProvider
    : IDisposable, IReadOnlyCollection<EntryModel>
{
    /// <summary>
    /// Extracts an entry in this provider to a file. If the file already exists, it is overwritten.
    /// </summary>
    /// <param name="entry">The file entry to extract.</param>
    /// <param name="destinationFileName">
    /// The path of the file to create from the contents of the entry.
    /// You can specify either a relative or an absolute path.
    /// A relative path is interpreted as relative to the current working directory.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the entry type is not <seealso cref="EntryType.File"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the entry type does not exist in the provider.
    /// </exception>
    /// <exception cref="PathTooLongException">
    /// Thrown if the specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if the specified path is invalid, such as being on an unmapped drive or a directory that does not exist.
    /// </exception>
    /// <exception cref="IOException">
    /// Thrown if an I/O error occurs, such as a disk error or the file being in use.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the caller does not have the required permission to access the file or directory.
    /// </exception>
    /// <exception cref="InvalidDataException">
    /// Thrown if the entry data is invalid or corrupted, preventing extraction.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the entry provider has been disposed and is no longer usable.
    /// </exception>
    void ExtractToFile(EntryModel entry, string destinationFileName);
}
