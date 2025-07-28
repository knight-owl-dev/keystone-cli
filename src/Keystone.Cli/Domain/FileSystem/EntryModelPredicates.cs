namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// Standard predicates for filtering <see cref="EntryModel"/> instances.
/// </summary>
public static class EntryModelPredicates
{
    /// <summary>
    /// Accepts all entries without filtering.
    /// </summary>
    /// <param name="_">The entry model.</param>
    /// <returns>
    /// Always <c>true</c> for all entries.
    /// </returns>
    public static bool AcceptAll(EntryModel _)
        => true;
}
