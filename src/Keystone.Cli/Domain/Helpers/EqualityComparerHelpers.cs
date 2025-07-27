namespace Keystone.Cli.Domain.Helpers;

/// <summary>
/// A collection of helper methods for working with equality comparers.
/// </summary>
public static class EqualityComparerHelpers
{
    /// <summary>
    /// Checks if two collections of items are equal based on reference equality and sequence equality.
    /// </summary>
    /// <remarks>
    /// The actual type of each collection is not enforced, so this method can be used with any type
    /// of collection that implements <see cref="IReadOnlyCollection{T}"/>.
    /// </remarks>
    /// <param name="a">A collection of items.</param>
    /// <param name="b">A collection of items.</param>
    /// <typeparam name="TItem">Type of items in the collection.</typeparam>
    /// <returns>
    /// <c>true</c> if both collections are the same reference or contain the same items in the same order;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool Equals<TItem>(IReadOnlyCollection<TItem>? a, IReadOnlyCollection<TItem>? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Count == b.Count && a.SequenceEqual(b);
    }
}
