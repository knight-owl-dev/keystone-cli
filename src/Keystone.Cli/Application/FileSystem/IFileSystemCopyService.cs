using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.Application.FileSystem;

/// <summary>
/// A service for copying files and directories within the file system using the <see cref="EntryModel"/>.
/// </summary>
public interface IFileSystemCopyService
{
    /// <summary>
    /// Copy files and directories from the source to the destination path.
    /// </summary>
    /// <remarks>
    /// If the destination path does not exist, it is created.
    /// </remarks>
    /// <param name="entryCollection">The source entry provider.</param>
    /// <param name="destinationPath">The destination path.</param>
    /// <param name="overwrite">Indicates if overwriting existing files is permitted.</param>
    /// <param name="predicate">
    /// If provided, the file system entries predicate implementing acceptance criteria
    /// for file paths relative to the repository root.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entryCollection"/> or <paramref name="destinationPath"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destinationPath"/> is an empty string.
    /// </exception>
    void Copy(
        IEntryCollection entryCollection,
        string destinationPath,
        bool overwrite,
        Func<EntryModel, bool>? predicate = null
    );
}
