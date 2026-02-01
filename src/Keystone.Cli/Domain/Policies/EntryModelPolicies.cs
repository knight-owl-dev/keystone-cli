using System.Collections.Immutable;
using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.Domain.Policies;

/// <summary>
/// <see cref="EntryModel"/> policies for filtering project content when copying
/// files from templates.
/// </summary>
public static class EntryModelPolicies
{
    /// <summary>
    /// A set of file names that are considered Git-related files.
    /// </summary>
    private static readonly ImmutableHashSet<string> GitFiles
        = [".gitignore", ".gitattributes", ".gitkeep", ".gitmodules"];

    /// <summary>
    /// A set of directory names that are considered user content directories.
    /// </summary>
    private static readonly ImmutableHashSet<string> UserContentDirectories
        = ["appendix", "artifacts", "assets", "chapters", "drafts", "research"];

    /// <summary>
    /// Determines whether the specified entry should be included when ignoring Git-related
    /// files and directories.
    /// </summary>
    /// <remarks>
    /// This policy excludes <c>.gitignore</c>, <c>.gitattributes</c>, <c>.gitkeep</c>, <c>.gitmodules</c>,
    /// and the <c>.git</c> directory itself.
    /// </remarks>
    /// <param name="entry">The source entry.</param>
    /// <returns>
    /// <c>true</c> if the entry is not a Git-related file or directory;
    /// <c>false</c> otherwise.
    /// </returns>
    public static bool ExcludeGitContent(EntryModel entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return entry.Type switch
        {
            EntryType.File => !GitFiles.Contains(entry.Name),
            EntryType.Directory => entry.DirectoryName != ".git",
            _ => true,
        };
    }

    /// <summary>
    /// Determines whether the specified entry should be included when ignoring user content.
    /// </summary>
    /// <param name="entry">The source entry.</param>
    /// <returns>
    /// <c>true</c> if the entry is not a user content directory; <c>false</c> otherwise.
    /// </returns>
    public static bool ExcludeUserContent(EntryModel entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return entry.Type switch
        {
            EntryType.File => !entry.IsInAnyDirectory(UserContentDirectories),
            EntryType.Directory => !UserContentDirectories.Contains(entry.DirectoryName),
            _ => true,
        };
    }

    /// <summary>
    /// Excludes both Git-related files and user content directories.
    /// </summary>
    /// <param name="entry">The source entry.</param>
    /// <returns>
    /// <c>true</c> if the entry is neither a Git-related file nor a user content directory;
    /// <c>false</c> otherwise.
    /// </returns>
    public static bool ExcludeGitAndUserContent(EntryModel entry)
        => ExcludeGitContent(entry) && ExcludeUserContent(entry);
}
