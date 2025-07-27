using System.Text;
using Keystone.Cli.Domain.Helpers;


namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// Represents a node in a hierarchical file system entry tree, where each node wraps a file
/// or directory entry and may contain child nodes if the entry is a directory.
/// </summary>
/// <param name="entry">Either file or directory entry.</param>
public class EntryNode(EntryModel entry)
{
    /// <summary>
    /// Gets the file system entry model associated with this node.
    /// </summary>
    public EntryModel Entry => entry;

    /// <summary>
    /// Gets the list of child nodes for this entry, or <c>null</c> if the entry is a file.
    /// </summary>
    private List<EntryNode>? Children { get; } = entry.Type == EntryType.Directory ? [] : null;

    /// <summary>
    /// Returns a string representation of this node and its descendants.
    /// </summary>
    /// <returns>
    /// The string representation of this entry node, including its relative path and
    /// the relative paths of its children (if any).
    /// </returns>
    public override string ToString()
        => Aggregate(
            new StringBuilder(),
            predicate: _ => true,
            (acc, item) => acc.AppendLine(item.RelativePath),
            acc => acc.ToString()
        );

    /// <summary>
    /// Gets the hash code for this entry node based on its entry model.
    /// </summary>
    /// <returns>
    /// The hash code for this entry node.
    /// </returns>
    public override int GetHashCode()
        => entry.GetHashCode();

    /// <summary>
    /// Checks if this entry node is equal to another object.
    /// </summary>
    /// <param name="obj">Another object.</param>
    /// <returns>
    /// <c>true</c> if the other object is an <see cref="EntryNode"/> with the same entry and children (when applicable);
    /// otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (EntryNode) obj;

        return Equals(this.Entry, other.Entry)
            && EqualityComparerHelpers.Equals(this.Children, other.Children);
    }

    /// <summary>
    /// Adds a child node to this entry node.
    /// </summary>
    /// <param name="child">A child node.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this node does not represent a directory.
    /// </exception>
    public void AddChild(EntryNode child)
    {
        if (this.Children is null)
        {
            throw new InvalidOperationException("Cannot add children to a file entry.");
        }

        this.Children.Add(child);
    }

    /// <summary>
    /// Recursively aggregates the entries in this tree that satisfy the specified predicate.
    /// </summary>
    /// <param name="seed">The initial result value.</param>
    /// <param name="predicate">The predicate for selecting entries.</param>
    /// <param name="func">The accumulator function.</param>
    /// <param name="resultSelector">The result selector function.</param>
    /// <typeparam name="TAccumulator">Type of result.</typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns>
    /// The aggregated result based on the provided seed, predicate, and result selector.
    /// If the predicate does not match the entry, the seed is returned unchanged.
    /// </returns>
    public TResult Aggregate<TAccumulator, TResult>(
        TAccumulator seed,
        Func<EntryModel, bool> predicate,
        Func<TAccumulator, EntryModel, TAccumulator> func,
        Func<TAccumulator, TResult> resultSelector
    )
        => resultSelector(Aggregate(seed, predicate, func));

    /// <summary>
    /// Recursively applies an accumulator function to all entries in this tree that satisfy the specified predicate.
    /// </summary>
    /// <typeparam name="TAccumulate">The type of the accumulated result.</typeparam>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="predicate">A predicate used to determine which entries should be processed.</param>
    /// <param name="func">The accumulator function to apply to each matching entry.</param>
    /// <returns>
    /// The final accumulated result. If a node does not satisfy the predicate, it and all of its children are skipped.
    /// </returns>
    public TAccumulate Aggregate<TAccumulate>(
        TAccumulate seed,
        Func<EntryModel, bool> predicate,
        Func<TAccumulate, EntryModel, TAccumulate> func
    )
    {
        if (! predicate(entry))
        {
            return seed;
        }

        if (this.Children is null)
        {
            return func(seed, entry);
        }

        return this.Children.Aggregate(
            new
            {
                Result = func(seed, entry),
                Predicate = predicate,
                ResultSelector = func,
            },
            (acc, child) => acc with
            {
                Result = child.Aggregate(acc.Result, acc.Predicate, acc.ResultSelector),
            },
            acc => acc.Result
        );
    }

    /// <summary>
    /// Creates a hierarchical <see cref="EntryNode"/> tree from a flat collection of <see cref="EntryModel"/> entries,
    /// without assuming any specific input order.
    /// </summary>
    /// <remarks>
    /// This method constructs a tree structure where each node wraps a file or directory entry.
    /// Directory nodes are connected to their children based on their relative paths.
    /// Entries must have consistent and valid relative paths that imply their position in the hierarchy.
    /// </remarks>
    /// <param name="entries">A flat collection of entry models representing both files and directories.</param>
    /// <returns>
    /// A root <see cref="EntryNode"/> instance representing the top-level (virtual) directory containing all provided entries.
    /// </returns>
    public static EntryNode CreateNode(IEnumerable<EntryModel> entries)
    {
        throw new NotImplementedException();
    }
}
