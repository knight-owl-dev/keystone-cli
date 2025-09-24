using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;


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
public static partial class YamlParsingUtility
{
    /// <summary>
    /// Parses a collection of YAML lines into structured entries.
    /// </summary>
    /// <param name="lines">All lines from a fully loaded YAML file.</param>
    /// <returns>A collection of parsed <see cref="Entry"/> values.</returns>
    public static IEnumerable<Entry> Parse(IEnumerable<string> lines)
        => lines.Aggregate(
            seed: (
                Entries: new List<Entry>(),
                Buffer: new List<string>(),
                Mode: EntryParsingMode.Unknown
            ),
            (acc, line) =>
            {
                var (entries, buffer, mode) = acc;

                var nextMode = IsBlankLineOrComment(line)
                    ? EntryParsingMode.Unknown
                    : EntryParsingMode.Yaml;

                var shouldFlush = buffer.Count > 0
                    && (mode != nextMode || (mode == EntryParsingMode.Yaml && IsTopLevelKeyOrTerminator(line)));

                if (shouldFlush)
                {
                    entries.Add(ParseBuffer(buffer.ToArray(), mode));
                    buffer.Clear();
                }

                buffer.Add(line);

                return acc with
                {
                    Mode = nextMode,
                };
            },
            acc =>
            {
                if (acc.Buffer.Count > 0)
                {
                    acc.Entries.Add(ParseBuffer(acc.Buffer.ToArray(), acc.Mode));
                }

                return acc.Entries;
            }
        );

    /// <summary>
    /// Prepares a <see cref="ScalarEntry"/> from a string value.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="value">A scalar value.</param>
    /// <returns>
    /// A <see cref="ScalarEntry"/> representing the provided value.
    /// </returns>
    public static ScalarEntry ToScalarEntry(string propertyName, string? value)
        => new(propertyName, value, [..WriteYamlFragment(propertyName, value)]);

    /// <summary>
    /// Prepares an <see cref="ArrayEntry"/> from a collection of string items.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="items">A collection of string items.</param>
    /// <returns>
    /// A <see cref="ArrayEntry"/> representing the provided items.
    /// </returns>
    public static ArrayEntry ToArrayEntry(string propertyName, IReadOnlyList<string> items)
        => new(propertyName, items, [..WriteYamlFragment(propertyName, items)]);

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
    public sealed record ScalarEntry(string PropertyName, string? Value, string[] RawLines) : Entry(PropertyName, RawLines, EntryKind.Scalar)
    {
        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(this.PropertyName, this.Value);

        /// <inheritdoc />
        public bool Equals(ScalarEntry? other)
            => other is not null
                && this.PropertyName == other.PropertyName
                && this.Value == other.Value
                && this.RawLines.SequenceEqual(other.RawLines);
    }

    /// <summary>
    /// The YAML array entry type.
    /// </summary>
    /// <param name="PropertyName">The property name.</param>
    /// <param name="Items">A collection of items.</param>
    /// <param name="RawLines">Raw lines representing the serialized entry.</param>
    public sealed record ArrayEntry(string PropertyName, IReadOnlyList<string> Items, string[] RawLines) : Entry(PropertyName, RawLines, EntryKind.Array)
    {
        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(this.PropertyName, this.Items.Aggregate(0, HashCode.Combine));

        /// <inheritdoc />
        public bool Equals(ArrayEntry? other)
            => other is not null
                && this.PropertyName == other.PropertyName
                && this.Items.SequenceEqual(other.Items)
                && this.RawLines.SequenceEqual(other.RawLines);
    }

    /// <summary>
    /// The YAML unknown entry type, used for lines that do not conform to expected formats.
    /// </summary>
    /// <param name="RawLines">Raw lines representing the serialized entry.</param>
    public sealed record UnknownEntry(string[] RawLines) : Entry(PropertyName: null, RawLines, EntryKind.Unknown)
    {
        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(this.RawLines.Aggregate(0, HashCode.Combine));

        /// <inheritdoc />
        public bool Equals(UnknownEntry? other)
            => other is not null && this.RawLines.SequenceEqual(other.RawLines);
    }

    /// <summary>
    /// Determines whether a line is blank or a comment.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>
    /// <c>true</c> if the line starts with <c>#</c> (after trimming leading whitespace)
    /// or is empty/whitespace; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsBlankLineOrComment(string line)
        => string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#');

    /// <summary>
    /// Determines whether a line is a top-level YAML key (not indented and contains a colon)
    /// or a YAML document terminator line (<c>...</c>).
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>
    /// <c>true</c> if the line is a top-level key or a terminator; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsTopLevelKeyOrTerminator(string line)
        => TopLevelKeyRx().IsMatch(line) || IsTerminatorLine(line);

    /// <summary>
    /// A regex to detect top-level YAML keys (not indented and contains a colon).
    /// </summary>
    [GeneratedRegex(@"^([^\s:#]+):(?:\s|$)")]
    private static partial Regex TopLevelKeyRx();

    /// <summary>
    /// Entry parsing modes to guide how buffered lines are interpreted.
    /// </summary>
    private enum EntryParsingMode
    {
        /// <summary>
        /// Building an unknown entry (e.g. comments, blank lines, or unrecognized formats).
        /// </summary>
        Unknown,

        /// <summary>
        /// Building a YAML entry (either scalar or array).
        /// </summary>
        Yaml,
    }

    /// <summary>
    /// The YAML deserializer instance configured to ignore unmatched properties.
    /// </summary>
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// The YAML serializer instance used for generating top-level YAML fragments.
    /// </summary>
    /// <remarks>
    /// The serializer must retain all properties, including those with default values, to avoid accidental removal of keys.
    /// Since comments are preserved as part of raw lines, omitting a key (due to a default value) may orphan comments or
    /// shift them to unrelated entries, leading to a loss of context. This serializer is configured to preserve structure
    /// faithfully and should not omit defaults.
    /// </remarks>
    private static readonly ISerializer YamlSerializer = new SerializerBuilder()
        .WithEventEmitter(nextEmitter => new LiteralMultilineStyleEmitter(nextEmitter))
        .Build();

    private static Entry ParseBuffer(string[] buffer, EntryParsingMode mode)
    {
        if (mode == EntryParsingMode.Unknown)
        {
            return new UnknownEntry(buffer);
        }

        try
        {
            var yamlText = string.Join(Environment.NewLine, buffer);
            var yamlObject = YamlDeserializer.Deserialize<Dictionary<string, object?>>(yamlText);

            if (yamlObject is null or { Count: 0 } or { Count: > 1 })
            {
                return new UnknownEntry(buffer);
            }

            var propertyName = yamlObject.Keys.First();

            return yamlObject[propertyName] switch
            {
                null => new ScalarEntry(propertyName, null, buffer),
                string value => new ScalarEntry(propertyName, value, buffer),
                int value => new ScalarEntry(propertyName, Convert.ToString(value), buffer),
                bool value => new ScalarEntry(propertyName, Convert.ToString(value), buffer),
                IEnumerable<object?> values => new ArrayEntry(
                    propertyName,
                    [..values.Select(value => Convert.ToString(value) ?? string.Empty)],
                    buffer
                ),
                _ => new UnknownEntry(buffer),
            };
        }
        catch (YamlException)
        {
            return new UnknownEntry(buffer);
        }
    }

    /// <summary>
    /// Writes a YAML fragment for a given property name and value and returns the resulting YAML lines.
    /// </summary>
    /// <remarks>
    /// The fragment is serialized as a top-level YAML document with a single property.
    /// </remarks>
    /// <param name="propertyName">The property name.</param>
    /// <param name="value">The original.</param>
    /// <returns>
    /// Raw lines representing the serialized YAML fragment.
    /// </returns>
    private static IEnumerable<string> WriteYamlFragment(string propertyName, object? value)
    {
        var yamlObject = new Dictionary<string, object?>
        {
            [propertyName] = value,
        };

        using var writer = new StringWriter();
        YamlSerializer.Serialize(writer, yamlObject);

        using var reader = new StringReader(writer.ToString());
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }

    /// <summary>
    /// Forces multiline string values to be emitted using the literal block style (using the <c>|</c> indicator).
    /// </summary>
    /// <param name="nextEmitter">The next emitter in the chain.</param>
    internal sealed class LiteralMultilineStyleEmitter(IEventEmitter nextEmitter) : ChainedEventEmitter(nextEmitter)
    {
        /// <inheritdoc />
        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            ScalarEventInfo overrideEventInfo;
            if (eventInfo.Source.Type == typeof(string) && eventInfo.Source.Value is string text && text.Contains('\n'))
            {
                overrideEventInfo = new ScalarEventInfo(eventInfo.Source)
                {
                    IsPlainImplicit = false,
                    Style = ScalarStyle.Literal, // forces |
                };
            }
            else
            {
                overrideEventInfo = eventInfo;
            }

            base.Emit(overrideEventInfo, emitter);
        }
    }
}
