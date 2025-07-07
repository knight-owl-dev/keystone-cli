using System.Collections;
using System.Collections.Immutable;
using System.IO.Compression;
using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.Application.GitHub;

/// <summary>
/// The GitHub zip entry provider.
/// </summary>
public sealed class GitHubZipEntryProvider
    : IEntryProvider
{
    /// <summary>
    /// The zip archive downloaded from a GitHub project.
    /// </summary>
    private readonly ZipArchive _archive;

    /// <summary>
    /// Entry bindings that map <see cref="EntryModel"/> to <see cref="ZipArchiveEntry"/>,
    /// stored in the same order as they appear in the zip archive.
    /// </summary>
    private ImmutableList<EntryBinding> Bindings { get; }

    /// <summary>
    /// Maps <see cref="EntryModel"/> to <see cref="ZipArchiveEntry"/> for quick access to entries in the zip archive.
    /// </summary>
    private ImmutableDictionary<EntryModel, ZipArchiveEntry> Entries { get; }

    /// <summary>
    /// Gets the total count of entries in the zip archive.
    /// </summary>
    public int Count => this.Bindings.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubZipEntryProvider"/> class.
    /// </summary>
    /// <param name="archive">The zip archive downloaded from a GitHub project.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="archive"/> is <c>null</c>.
    /// </exception>
    public GitHubZipEntryProvider(ZipArchive archive)
    {
        ArgumentNullException.ThrowIfNull(archive);
        _archive = archive;

        this.Bindings = GetBindings(archive);

        this.Entries = this.Bindings.ToImmutableDictionary(
            binding => binding.EntryModel,
            binding => binding.ArchiveEntry
        );
    }

    /// <inheritdoc />
    public void Dispose()
        => Dispose(true);

    /// <summary>
    /// Disposes the resources used by the <see cref="GitHubZipEntryProvider"/>.
    /// </summary>
    /// <param name="disposing">Indicates if disposing explicitly.</param>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _archive.Dispose();
        }
    }

    /// <inheritdoc />
    public IEnumerator<EntryModel> GetEnumerator()
        => this.Bindings.Select(binding => binding.EntryModel).GetEnumerator();

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
    /// Immutable list of bindings <see cref="EntryModel"/> to <see cref="ZipArchiveEntry"/>.
    /// </returns>
    private static ImmutableList<EntryBinding> GetBindings(ZipArchive archive)
        => archive.Entries.Skip(1).Aggregate(
            new
            {
                RootEntry = archive.Entries[0],
                Builder = ImmutableList.CreateBuilder<EntryBinding>(),
            },
            (acc, archiveEntry) =>
            {
                var entryModel = new EntryModel(
                    GetEntryType(archiveEntry),
                    Path.GetFileName(archiveEntry.Name),
                    MakeRelative(acc.RootEntry, archiveEntry.FullName)
                );

                acc.Builder.Add(new EntryBinding(entryModel, archiveEntry));

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

    /// <summary>
    /// Determines the type of the entry.
    /// </summary>
    /// <remarks>
    /// GitHub zip entries ending with '/' are directories, others are files.
    /// </remarks>
    /// <param name="entry">The zip archive entry.</param>
    /// <returns>
    /// The type of the entry, either <see cref="EntryType.File"/> or <see cref="EntryType.Directory"/>.
    /// </returns>
    private static EntryType GetEntryType(ZipArchiveEntry entry)
        => entry.FullName.EndsWith('/') ? EntryType.Directory : EntryType.File;

    /// <summary>
    /// Entry biding that maps <see cref="EntryModel"/> to <see cref="ZipArchiveEntry"/>.
    /// </summary>
    /// <param name="EntryModel">The entry model.</param>
    /// <param name="ArchiveEntry">The zip archive entry.</param>
    private record EntryBinding(EntryModel EntryModel, ZipArchiveEntry ArchiveEntry);
}
