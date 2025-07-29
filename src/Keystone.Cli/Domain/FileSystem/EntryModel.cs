namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// The file system entry model.
/// </summary>
/// <remarks>
/// Use the <see cref="Create(string)"/> method to create an instance of this model.
/// </remarks>
/// <param name="Type">The type of entry.</param>
/// <param name="Name">The name of the entry. Empty string for a directory.</param>
/// <param name="RelativePath">Relative path for this entry. Ends with a <see cref="DirectorySeparatorChar"/> for a directory.</param>
public record EntryModel(EntryType Type, string Name, string RelativePath)
{
    /// <summary>
    /// The only valid directory separator char for relative paths in this model.
    /// </summary>
    private const char DirectorySeparatorChar = '/';

    /// <summary>
    /// Returns the directory name for this entry.
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
    public string DirectoryName { get; } = Path.GetDirectoryName(RelativePath)!;

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
    /// Creates an <see cref="EntryModel"/> based on the provided relative path.
    /// </summary>
    /// <param name="relativePath">
    /// The relative path to either a file or directory. Must use <see cref="DirectorySeparatorChar"/> in composite paths,
    /// e.g., <c>"A/B/C.txt"</c> for a file or <c>"A/B/"</c> for a directory.
    /// </param>
    /// <returns>
    /// The <see cref="EntryModel"/> representing either a file or directory entry.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="relativePath"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="relativePath"/> is empty, uses invalid path separator or begins with <see cref="DirectorySeparatorChar"/>.
    /// </exception>
    public static EntryModel Create(string relativePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(relativePath);

        if (relativePath.StartsWith(DirectorySeparatorChar))
        {
            throw new ArgumentException(
                $"The relative path must not start with '{DirectorySeparatorChar}'.",
                nameof(relativePath)
            );
        }

        if (relativePath.Contains('\\'))
        {
            throw new ArgumentException(
                $"The relative path must use '{DirectorySeparatorChar}' as the directory separator.",
                nameof(relativePath)
            );
        }

        return relativePath.EndsWith(DirectorySeparatorChar)
            ? Directory(relativePath)
            : File(relativePath);
    }

    private static EntryModel File(string relativePath)
        => new(EntryType.File, Name: Path.GetFileName(relativePath), relativePath);

    private static EntryModel Directory(string relativePath)
        => new(EntryType.Directory, Name: string.Empty, RelativePath: relativePath);
}
