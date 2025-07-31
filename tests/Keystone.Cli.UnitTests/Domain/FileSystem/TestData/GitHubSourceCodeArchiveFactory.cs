using System.IO.Compression;
using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.UnitTests.Domain.FileSystem.TestData;

/// <summary>
/// Provides utilities to construct in-memory ZIP archives that mimic GitHub source code archives.
/// Intended for use in unit tests and fixtures where ZIP structure and contents need to be emulated.
/// </summary>
public static class GitHubSourceCodeArchiveFactory
{
    /// <summary>
    /// Creates an empty in-memory <see cref="ZipArchive"/> with a default root directory name of <c>"project"</c>.
    /// This is useful for tests that require a ZIP archive but do not need any specific entries.
    /// </summary>
    /// <returns>
    /// An empty <see cref="ZipArchive"/> opened in read mode, with a root directory named <c>"project"</c>.
    /// </returns>
    public static ZipArchive CreateEmpty()
        => Create(rootDirectoryName: "project", entries: []);

    /// <summary>
    /// Creates an in-memory <see cref="ZipArchive"/> resembling a GitHub-style source code ZIP archive,
    /// with a single root directory and a set of provided entries (files and subdirectories).
    /// </summary>
    /// <param name="rootDirectoryName">
    /// The name of the root directory (e.g., <c>repo-main</c>) that all entries will be nested under.
    /// Must not be rooted or end in a directory separator.
    /// </param>
    /// <param name="entries">
    /// A collection of <see cref="EntryModel"/> items representing files or directories to include in the archive,
    /// e.g., <see cref="StandardProjectLayout.MinimalTemplateEntries"/> or <see cref="StandardProjectLayout.FullTemplateEntries"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ZipArchive"/> opened in read mode, ready for test validation or inspection.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="rootDirectoryName"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the root directory name is empty, rooted or ends in a directory separator.
    /// </exception>
    public static ZipArchive Create(string rootDirectoryName, IEnumerable<EntryModel> entries)
    {
        ArgumentNullException.ThrowIfNull(rootDirectoryName);

        if (Path.IsPathRooted(rootDirectoryName))
        {
            throw new ArgumentException("Root directory name must not be rooted.", nameof(rootDirectoryName));
        }

        if (Path.EndsInDirectorySeparator(rootDirectoryName))
        {
            throw new ArgumentException("The root directory name must not end with a directory separator.", nameof(rootDirectoryName));
        }

        var memoryStream = new MemoryStream();

        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            // GitHub zip archives always start with a root directory.
            var rootZipEntry = archive.CreateEntry($"{rootDirectoryName}/", CompressionLevel.NoCompression);

            foreach (var entry in entries)
            {
                var entryName = entry.GetFullPath(rootZipEntry.FullName);
                var zipEntry = archive.CreateEntry(entryName, CompressionLevel.NoCompression);

                if (entry.Type == EntryType.Directory)
                {
                    continue;
                }

                using var entryStream = zipEntry.Open();
                using var writer = new StreamWriter(entryStream);

                writer.WriteLine($"# {zipEntry.FullName} file content");
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return new ZipArchive(memoryStream, ZipArchiveMode.Read);
    }
}
