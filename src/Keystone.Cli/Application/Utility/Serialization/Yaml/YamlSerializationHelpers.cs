using Keystone.Cli.Application.Utility.Text;


namespace Keystone.Cli.Application.Utility.Serialization.Yaml;

/// <summary>
/// Common utility methods for working with <see cref="YamlValue"/> instances.
/// </summary>
public static class YamlSerializationHelpers
{
    /// <summary>
    /// Checks whether a parsed YAML entry is equal to a given value.
    /// </summary>
    /// <param name="entry">The parsed YAML entry.</param>
    /// <param name="value">The YAML value.</param>
    /// <returns>
    /// <c>true</c> if the entry and value are considered equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEqualValue(YamlParsingUtility.Entry entry, YamlValue value)
        => (entry, value) switch
        {
            (YamlParsingUtility.ScalarEntry scalarEntry, YamlScalar scalarValue) => scalarEntry.Value == scalarValue.Value,
            (YamlParsingUtility.ArrayEntry arrayEntry, YamlArray arrayValue) => arrayEntry.Items.SequenceEqual(arrayValue.Items),
            _ => false,
        };

    /// <summary>
    /// Creates a YAML entry from a property name and value.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="value">The new value.</param>
    /// <returns>
    /// <c>true</c> if the entry and value are considered equal; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the value type is not supported.
    /// </exception>
    public static YamlParsingUtility.Entry ToEntry(string propertyName, YamlValue value)
        => AsEntry(new KeyValuePair<string, YamlValue>(propertyName, value));

    /// <summary>
    /// Converts a key-value pair to a YAML entry.
    /// </summary>
    /// <param name="kvp">The key-value pair.</param>
    /// <returns>
    /// The corresponding <see cref="YamlParsingUtility.Entry"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the value type is not supported.
    /// </exception>
    public static YamlParsingUtility.Entry AsEntry(KeyValuePair<string, YamlValue> kvp)
        => kvp switch
        {
            (var key, YamlScalar scalar) => YamlParsingUtility.ToScalarEntry(key, scalar.Value),
            (var key, YamlArray array) => YamlParsingUtility.ToArrayEntry(key, array.Items),
            var (key, unknown) => throw new NotSupportedException($"Unsupported YAML property '{key}' of value type: {unknown.GetType().FullName}"),
        };

    /// <summary>
    /// Gets a <see cref="YamlValue"/> from a parsed YAML entry.
    /// </summary>
    /// <param name="entry">Parsed YAML entry.</param>
    /// <returns>
    /// The corresponding <see cref="YamlValue"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if the entry type is not supported.
    /// </exception>
    public static YamlValue GetValue(YamlParsingUtility.Entry entry)
        => entry switch
        {
            YamlParsingUtility.ScalarEntry scalarEntry => new YamlScalar(scalarEntry.Value),
            YamlParsingUtility.ArrayEntry arrayEntry => new YamlArray(arrayEntry.Items),
            _ => throw new NotSupportedException($"Unsupported YAML entry type: {entry.GetType().FullName}"),
        };
}
