namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// Defines an entry provider that supplies file system entries (files or directories) for reading or extraction.
/// </summary>
public interface IEntryProvider
    : IDisposable, IReadOnlyCollection<EntryModel>
{
    /// <summary>
    /// Copies the specified file entry to the provided destination stream.
    /// </summary>
    /// <param name="entry">The file entry to copy.</param>
    /// <param name="destination">The destination stream to which the entry's content will be written.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the copy operation if needed.</param>
    /// <returns>
    /// A task that represents the asynchronous copy operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the entry type is not a file or is unsupported for copying.
    /// </exception>
    Task CopyToAsync(EntryModel entry, Stream destination, CancellationToken cancellationToken = default);
}
