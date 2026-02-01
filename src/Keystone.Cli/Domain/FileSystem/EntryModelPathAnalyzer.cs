// CA1034: Analyzer doesn't recognize C# 13 extension block syntax

#pragma warning disable CA1034

namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// Provides path analysis methods for <see cref="EntryModel"/> instances.
/// </summary>
public static class EntryModelPathAnalyzer
{
    /// <param name="entry">The entry to analyze.</param>
    extension(EntryModel entry)
    {
        /// <summary>
        /// Determines whether the file entry is a direct child of the specified top-level directory.
        /// </summary>
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
        public bool IsDirectChildOf(string directoryName)
        {
            ArgumentNullException.ThrowIfNull(directoryName);

            return entry.Type == EntryType.File
                && !string.IsNullOrEmpty(directoryName)
                && entry.RelativePath.StartsWith($"{directoryName}/", StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Determines whether the file entry is a direct child of the specified top-level directories.
        /// </summary>
        /// <param name="directoryNames">The collection of top-level directory names to check against.</param>
        /// <returns>
        /// <c>true</c> if the entry is a file that is a direct child of the specified directories;
        /// <c>false</c> otherwise.
        /// </returns>
        public bool IsInAnyDirectory(IEnumerable<string> directoryNames)
        {
            ArgumentNullException.ThrowIfNull(directoryNames);

            return entry.Type == EntryType.File && directoryNames.Any(entry.IsDirectChildOf);
        }
    }
}
