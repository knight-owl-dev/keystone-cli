using System.Collections.Immutable;
using System.Text;
using JetBrains.Annotations;
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
    private EntryModel Entry => entry;

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
            EntryModelPredicates.AcceptAll,
            (acc, item) => acc.AppendLine(item.RelativePath),
            acc => acc.ToString().TrimEnd()
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
    /// <returns>
    /// Returns this node after adding the child to its list of children.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the child node is the same as this node.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this node does not represent a directory.
    /// </exception>
    public EntryNode AddChild(EntryNode child)
    {
        if (this.Children is null)
        {
            throw ErrorAddChildToFileEntry();
        }

        if (ReferenceEquals(this, child))
        {
            throw ErrorAddSelfAsChild(nameof(child));
        }

        this.Children.Add(child);

        return this;
    }

    /// <summary>
    /// Adds multiple child nodes to this entry node.
    /// </summary>
    /// <param name="children">A collection of children nodes.</param>
    /// <returns>
    /// Returns this node after adding the children to its list of children.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if any child node is the same as this node.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this node does not represent a directory.
    /// </exception>
    public EntryNode AddChildren(IEnumerable<EntryNode> children)
    {
        if (this.Children is null)
        {
            throw ErrorAddChildToFileEntry();
        }

        var childrenAsList = children as IReadOnlyCollection<EntryNode> ?? children.ToList();
        if (childrenAsList.Any(child => ReferenceEquals(this, child)))
        {
            throw ErrorAddSelfAsChild(nameof(children));
        }

        this.Children.AddRange(childrenAsList);

        return this;
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
    /// Creates a forest of <see cref="EntryNode"/> trees from a flat collection of <see cref="EntryModel"/> entries,
    /// without assuming any specific input order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method reconstructs a hierarchy of entry nodes by analyzing the relative paths of files and directories.
    /// Each returned node represents a top-level directory or file. Child relationships are established by splitting
    /// the relative paths and connecting nodes accordingly.
    /// </para>
    /// <para>
    /// All directories end with <c>"/"</c>, which guarantees that grouping by <see cref="EntryModel.DirectoryName"/>
    /// yields exactly one directory per group, with its associated file entries. Consequently, top-level resolve their
    /// parent directory to an empty string.
    /// </para>
    /// <para>
    /// Using the <see cref="EntryModelSortPolicy.DirectoriesFirst"/> policy guarantees that parent directories are created
    /// before any of their children are processed. This ensures correct construction of the parent-child relationships
    /// as the tree is built incrementally one directory at a time.
    /// </para>
    /// </remarks>
    /// <param name="entries">A flat collection of entry models representing files and directories.</param>
    /// <returns>
    /// A collection of top-level <see cref="EntryNode"/> instances representing the entry trees.
    /// </returns>
    public static ImmutableList<EntryNode> CreateNodes(IEnumerable<EntryModel> entries)
        => EntryModelSortPolicy.DirectoriesFirst(entries).GroupBy(entry => entry.DirectoryName).Aggregate(
            new
            {
                TopLevelNodes = ImmutableList.CreateBuilder<EntryNode>(),
                Directories = new Dictionary<string, EntryNode>(),
            },
            (acc, group) =>
            {
                var directoryName = group.Key;
                var entriesByType = group.GroupBy(entry => entry.Type).ToImmutableDictionary(
                    entryTypeGroup => entryTypeGroup.Key,
                    entryTypeGroup => entryTypeGroup.ToList()
                );

                if (directoryName == string.Empty || ! entriesByType.TryGetValue(EntryType.Directory, out var directories))
                {
                    // all top-level files
                    acc.TopLevelNodes.AddRange(group.Select(entry => new EntryNode(entry)));
                    return acc;
                }

                var directory = new EntryNode(directories.Single());
                acc.Directories.Add(directoryName, directory);

                if (entriesByType.TryGetValue(EntryType.File, out var fileEntries))
                {
                    directory.AddChildren(fileEntries.Select(entry => new EntryNode(entry)));
                }

                switch (Path.GetDirectoryName(directoryName))
                {
                    case "":
                        acc.TopLevelNodes.Add(directory);
                        return acc;

                    case { } parentDirectoryName when acc.Directories.TryGetValue(parentDirectoryName, out var parentDirectory):
                        parentDirectory.AddChild(directory);
                        return acc;

                    default:
                        throw new InvalidOperationException($"Parent directory not found for: \"{directoryName}\".");
                }
            },
            acc => acc.TopLevelNodes.ToImmutable()
        );

    private static InvalidOperationException ErrorAddChildToFileEntry()
        => new("Cannot add children to a file entry.");

    private static ArgumentException ErrorAddSelfAsChild([InvokerParameterName] string paramName)
        => new("Adding self as a child is not allowed.", paramName);
}
