namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// Defines an entry provider that supplies file system entries (files or directories) for reading or extraction.
/// </summary>
public interface IEntryProvider
    : IDisposable, IReadOnlyCollection<EntryModel>
{
    /// <summary>
    /// Opens the file entry for reading.
    /// </summary>
    /// <param name="entry">The file entry to copy.</param>
    /// <returns>
    /// A task that represents the asynchronous copy operation.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the entry type is not <seealso cref="EntryType.File"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the entry type does not exist in the provider.
    /// </exception>
    Stream Open(EntryModel entry);
}
