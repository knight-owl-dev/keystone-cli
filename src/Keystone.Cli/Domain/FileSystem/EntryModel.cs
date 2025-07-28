namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// The file system entry model.
/// </summary>
/// <remarks>
/// Use the <see cref="Create(string)"/> method to create an instance of this model.
/// </remarks>
/// <param name="Type">The type of entry.</param>
/// <param name="Name">The name of the entry. Empty string for a directory.</param>
/// <param name="RelativePath">Relative path for this entry. Ends with a <see cref="PathSeparator"/> for a directory.</param>
public record EntryModel(EntryType Type, string Name, string RelativePath)
{
    /// <summary>
    /// The only valid path separator for relative paths in this model.
    /// </summary>
    public const char PathSeparator = '/';

    /// <summary>
    /// Gets the full path of the entry based on the provided root path.
    /// </summary>
    /// <param name="rootPath">The root path. May use OS-specific path delimiters.</param>
    /// <returns>
    /// The full path of the entry, combining the root path and the relative path.
    /// </returns>
    public string GetFullPath(string rootPath)
        => Path.Combine(rootPath, this.RelativePath);

    /// <summary>
    /// Returns the directory information for this entry.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For file entries, this returns the directory containing the file, e.g., for <c>"A/B/C.txt"</c> returns <c>"A/B"</c>.
    /// For the top-level file, this returns an empty string, e.g., for <c>"C.txt"</c> returns <c>""</c>.
    /// </para>
    /// <para>
    /// For directory entries, this returns the directory itself without the trailing path delimiter, e.g., for <c>"A/B/"</c> returns <c>"A/B"</c>.
    /// For a top-level directory, this still returns the directory itself, e.g., for <c>"A/"</c> returns <c>"A"</c>.
    /// </para>
    /// </remarks>
    /// <returns>
    /// The directory information for this entry.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when trying to get the directory name for the root entry (i.e., when <see cref="RelativePath"/> is <c>"/"</c>).
    /// </exception>
    public string GetDirectoryName()
        => Path.GetDirectoryName(this.RelativePath)
            ?? throw new InvalidOperationException("Cannot get directory name for the root entry.");

    /// <summary>
    /// Creates an <see cref="EntryModel"/> based on the provided relative path.
    /// </summary>
    /// <param name="relativePath">
    /// The relative path to either a file or directory. Must use <see cref="PathSeparator"/> in composite paths,
    /// e.g., <c>"A/B/C.txt"</c> for a file or <c>"A/B/"</c> for a directory.
    /// </param>
    /// <returns>
    /// The <see cref="EntryModel"/> representing either a file or directory entry.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="relativePath"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="relativePath"/> is empty or uses invalid path separator.
    /// </exception>
    public static EntryModel Create(string relativePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(relativePath);

        if (relativePath.Contains('\\'))
        {
            throw new ArgumentException(
                $"The relative path must use '{PathSeparator}' as the path separator.",
                nameof(relativePath)
            );
        }

        return relativePath.EndsWith(PathSeparator)
            ? Directory(relativePath)
            : File(relativePath);
    }

    private static EntryModel File(string relativePath)
        => new(EntryType.File, Name: Path.GetFileName(relativePath), relativePath);

    private static EntryModel Directory(string relativePath)
        => new(EntryType.Directory, Name: string.Empty, RelativePath: relativePath);
}
