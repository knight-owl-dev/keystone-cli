using System.Collections;
using System.Collections.Immutable;
using System.IO.Compression;
using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.Application.GitHub;

/// <summary>
/// The GitHub zip entry provider.
/// </summary>
public sealed class GitHubZipEntryProvider(ZipArchive archive)
    : IEntryProvider
{
    /// <summary>
    /// Maps <see cref="EntryModel"/> to <see cref="ZipArchiveEntry"/> for quick access to entries in the zip archive.
    /// </summary>
    private ImmutableDictionary<EntryModel, ZipArchiveEntry> Entries { get; } = GetEntries(archive);

    /// <summary>
    /// Gets the total count of entries in the zip archive.
    /// </summary>
    public int Count => this.Entries.Count;

    /// <inheritdoc />
    public void Dispose()
        => archive.Dispose();

    /// <inheritdoc />
    public IEnumerator<EntryModel> GetEnumerator()
        => this.Entries.Keys.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc />
    public void ExtractToFile(EntryModel entry, string destinationFileName)
    {
        if (entry.Type != EntryType.File)
        {
            throw new ArgumentException($"The entry type must be '{EntryType.File}'.", nameof(entry));
        }

        if (! this.Entries.TryGetValue(entry, out var archiveEntry))
        {
            throw new InvalidOperationException($"The '{entry.Name}' entry does not exist in the zip archive at {entry.RelativePath}.");
        }

        archiveEntry.ExtractToFile(destinationFileName, overwrite: true);
    }

    /// <summary>
    /// Creates a mapping of <see cref="EntryModel"/> to <see cref="ZipArchiveEntry"/> for the entries in the zip archive.
    /// </summary>
    /// <remarks>
    /// GitHub zips wrap content in a top-level folder, so the first entry is the root directory.
    /// </remarks>
    /// <param name="archive">The zip archive.</param>
    /// <returns>
    /// Immutable dictionary binding <see cref="EntryModel"/> to <see cref="ZipArchiveEntry"/>.
    /// </returns>
    private static ImmutableDictionary<EntryModel, ZipArchiveEntry> GetEntries(ZipArchive archive)
        => archive.Entries.Skip(1).Aggregate(
            new
            {
                RootEntry = archive.Entries[0],
                Builder = ImmutableDictionary.CreateBuilder<EntryModel, ZipArchiveEntry>(),
            },
            (acc, archiveEntry) =>
            {
                var relativePath = MakeRelative(acc.RootEntry, archiveEntry.FullName);
                var entryModel = EntryModel.Create(relativePath);

                acc.Builder.Add(entryModel, archiveEntry);

                return acc;
            },
            acc => acc.Builder.ToImmutable()
        );

    /// <summary>
    /// Makes a relative path from the root entry to the specified path.
    /// </summary>
    /// <param name="root">The root entry.</param>
    /// <param name="path">The full path inside the archive entry.</param>
    /// <returns>
    /// The relative path to the root entry.
    /// </returns>
    private static string MakeRelative(ZipArchiveEntry root, string path)
        => path.StartsWith(root.FullName) ? path[root.FullName.Length..] : path;
}
