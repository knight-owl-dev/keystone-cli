namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// The sort policy for <see cref="EntryModel"/> instances.
/// </summary>
public static class EntryModelSortPolicy
{
    /// <summary>
    /// Orders the entries such that directories come first, followed by files,
    /// and then sorts them by their relative paths in a case-sensitive manner.
    /// </summary>
    /// <param name="entries">A collection of entries to sort.</param>
    /// <returns>
    /// Ordered collection of <see cref="EntryModel"/> instances.
    /// </returns>
    public static IEnumerable<EntryModel> DirectoriesFirst(IEnumerable<EntryModel> entries)
        => entries
            .OrderBy(entry => entry.Type == EntryType.Directory ? 0 : 1)
            .ThenBy(entry => entry.RelativePath, StringComparer.Ordinal);
}
