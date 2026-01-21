using System.Collections;
using Keystone.Cli.Domain.FileSystem;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Domain.FileSystem;

/// <summary>
/// Fake implementation of <see cref="IEntryCollection"/> for easier testing.
/// </summary>
/// <param name="entries">A collection of entries.</param>
public sealed class EntryCollection(IReadOnlyCollection<EntryModel> entries)
    : IEntryCollection
{
    /// <summary>
    /// Fakes the <see cref="IEntryCollection"/> interface for testing purposes.
    /// </summary>
    /// <param name="entries">Stubbed entries to use with the fake.</param>
    /// <returns>
    /// A fake <see cref="IEntryCollection"/> that can be used in tests.
    /// </returns>
    public static IEntryCollection Fake(IReadOnlyCollection<EntryModel> entries)
        => Substitute.ForTypeForwardingTo<IEntryCollection, EntryCollection>(entries);

    /// <inheritdoc />
    public int Count => entries.Count;

    /// <inheritdoc />
    public void Dispose()
    {
        // do nothing
    }

    /// <inheritdoc />
    public IEnumerator<EntryModel> GetEnumerator()
        => entries.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc />
    public void ExtractToFile(EntryModel entry, string destinationFileName)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.Type != EntryType.File)
        {
            throw new ArgumentException($"The entry type must be '{EntryType.File}'.", nameof(entry));
        }

        if (! entries.Contains(entry))
        {
            throw new InvalidOperationException($"The '{entry.Name}' entry does not exist.");
        }
    }
}
