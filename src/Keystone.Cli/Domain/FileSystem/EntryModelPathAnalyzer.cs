namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// Provides path analysis methods for <see cref="EntryModel"/> instances.
/// </summary>
public static class EntryModelPathAnalyzer
{
    /// <summary>
    /// Determines whether the file entry is a direct child of the specified top-level directory.
    /// </summary>
    /// <param name="entry">The entry to analyze.</param>
    /// <param name="directoryName">The top-level directory name to check against.</param>
    /// <returns>
    /// <c>true</c> if the entry is a file that is a direct child of the specified directory;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <example>
    /// <para>For a directory name "artifacts":</para>
    /// <list type="bullet">
    /// <item><description>"artifacts/file.pdf" → <c>true</c></description></item>
    /// <item><description>"artifacts/sub/file.pdf" → <c>true</c></description></item>
    /// <item><description>"artifacts-backup/file.pdf" → <c>false</c></description></item>
    /// <item><description>"sub/artifacts/file.pdf" → <c>false</c></description></item>
    /// </list>
    /// </example>
    public static bool IsDirectChildOf(this EntryModel entry, string directoryName)
    {
        ArgumentNullException.ThrowIfNull(directoryName);

        return entry.Type == EntryType.File
            && ! string.IsNullOrEmpty(directoryName)
            && entry.RelativePath.StartsWith($"{directoryName}/");
    }

    /// <summary>
    /// Determines whether the file entry is a direct child of the specified top-level directories.
    /// </summary>
    /// <param name="entry">The entry to analyze.</param>
    /// <param name="directoryNames">The collection of top-level directory names to check against.</param>
    /// <returns>
    /// <c>true</c> if the entry is a file that is a direct child of the specified directories;
    /// <c>false</c> otherwise.
    /// </returns>
    public static bool IsInAnyDirectory(this EntryModel entry, IEnumerable<string> directoryNames)
    {
        ArgumentNullException.ThrowIfNull(directoryNames);

        return entry.Type == EntryType.File && directoryNames.Any(entry.IsDirectChildOf);
    }
}
