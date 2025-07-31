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
        => entry.Type switch
        {
            EntryType.File => ! GitFiles.Contains(entry.Name),
            EntryType.Directory => entry.DirectoryName != ".git",
            _ => true,
        };
}
