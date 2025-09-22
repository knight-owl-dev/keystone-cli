namespace Keystone.Cli.Application.Utility.Text;

/// <summary>
/// Common utility methods for parsing YAML text.
/// </summary>
/// <remarks>
/// <para>
/// This parsing utility is designed to preserve comments and formatting when reading and writing YAML files,
/// and is focused on basic key-value pairs including arrays without complex structures.
/// </para>
/// <para>
/// The choice to use <c>YamlDotNet</c> is a conscious and intentional tradeoff between performance and integrity.
/// By leveraging <c>YamlDotNet</c> for interpreting supported YAML constructs (such as scalars and sequences),
/// this utility avoids the need to re-implement the full YAML specification within the CLI tool. Structures not directly
/// supported are treated as unknown for simplicity and fidelity.
/// </para>
/// </remarks>
public static class YamlParsingUtility
{
    /// <summary>
    /// Parses a collection of YAML lines into structured entries.
    /// </summary>
    /// <param name="lines">All lines from a fully loaded YAML file.</param>
    /// <returns>A collection of parsed <see cref="Entry"/> values.</returns>
    public static IEnumerable<Entry> Parse(IReadOnlyCollection<string> lines)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Prepares a <see cref="ScalarEntry"/> from a string value.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="value">A scalar value.</param>
    /// <returns>
    /// A <see cref="ScalarEntry"/> representing the provided value.
    /// </returns>
    public static ScalarEntry ToScalarEntry(string propertyName, string? value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Prepares an <see cref="ArrayEntry"/> from a collection of string items.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="items">A collection of string items.</param>
    /// <returns>
    /// A <see cref="ArrayEntry"/> representing the provided items.
    /// </returns>
    public static ArrayEntry ToArrayEntry(string propertyName, IReadOnlyList<string> items)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Indicates whether the specified entry is a YAML document terminator entry (<c>...</c>).
    /// </summary>
    /// <param name="entry">A YAML entry.</param>
    /// <returns>
    /// <c>true</c> if the entry is a YAML document terminator; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsTerminatorEntry(Entry entry)
        => entry.Kind == EntryKind.Unknown && entry.RawLines.All(IsTerminatorLine);

    /// <summary>
    /// Indicates whether the specified line is a YAML document terminator line (<c>...</c>).
    /// </summary>
    /// <param name="line">A raw line from YAML document.</param>
    /// <returns>
    /// <c>true</c> if the line is a YAML document terminator; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsTerminatorLine(string line)
        => line == "...";

    /// <summary>
    /// Supported kinds of YAML entries for better code analyzer hints.
    /// </summary>
    public enum EntryKind
    {
        /// <summary>
        /// Use for simple key-value pairs.
        /// </summary>
        Scalar,

        /// <summary>
        /// Use for arrays (multiple items under a single key).
        /// </summary>
        Array,

        /// <summary>
        /// Use for lines that do not conform to expected formats.
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// The base type for all YAML entries.
    /// </summary>
    /// <param name="PropertyName">The property name.</param>
    /// <param name="RawLines">Raw lines representing the serialized entry.</param>
    /// <param name="Kind">The kind of YAML entry.</param>
    public abstract record Entry(string? PropertyName, string[] RawLines, EntryKind Kind)
    {
        /// <summary>
        /// Gets a string representation of the entry.
        /// </summary>
        /// <returns>
        /// <see cref="RawLines"/> as read from the source.
        /// </returns>
        public override string ToString()
            => string.Join(Environment.NewLine, this.RawLines);
    }

    /// <summary>
    /// The YAML scalar entry type.
    /// </summary>
    /// <remarks>
    /// If the scalar contains line breaks, it will be emitted as a literal block using the <c>|</c> syntax.
    /// Otherwise, it is emitted as a plain inline value.
    /// </remarks>
    /// <param name="PropertyName">The property name.</param>
    /// <param name="Value">The value.</param>
    /// <param name="RawLines">Raw lines representing the serialized entry.</param>
    public sealed record ScalarEntry(string PropertyName, string? Value, string[] RawLines) : Entry(PropertyName, RawLines, EntryKind.Scalar);

    /// <summary>
    /// The YAML array entry type.
    /// </summary>
    /// <param name="PropertyName">The property name.</param>
    /// <param name="Items">A collection of items.</param>
    /// <param name="RawLines">Raw lines representing the serialized entry.</param>
    public sealed record ArrayEntry(string PropertyName, IReadOnlyList<string> Items, string[] RawLines) : Entry(PropertyName, RawLines, EntryKind.Array);

    /// <summary>
    /// The YAML unknown entry type, used for lines that do not conform to expected formats.
    /// </summary>
    /// <param name="RawLines">Raw lines representing the serialized entry.</param>
    public sealed record UnknownEntry(string[] RawLines) : Entry(PropertyName: null, RawLines, EntryKind.Unknown);
}
