using System.Collections;
using Keystone.Cli.Domain.FileSystem;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Domain.FileSystem;

/// <summary>
/// Fake implementation of <see cref="IEntryProvider"/> for easier testing.
/// </summary>
/// <param name="entries">A collection of entries.</param>
public sealed class EntryProvider(IReadOnlyCollection<EntryModel> entries)
    : IEntryProvider
{
    /// <summary>
    /// Fakes the <see cref="IEntryProvider"/> interface for testing purposes.
    /// </summary>
    /// <param name="entries">Stubbed entries to use with the fake.</param>
    /// <returns>
    /// A fake <see cref="IEntryProvider"/> that can be used in tests.
    /// </returns>
    public static IEntryProvider Fake(IReadOnlyCollection<EntryModel> entries)
        => Substitute.ForTypeForwardingTo<IEntryProvider, EntryProvider>(entries);

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
