namespace Keystone.Cli.Application.Utility.Serialization.Yaml;

/// <summary>
/// YAML array value type.
/// </summary>
/// <param name="Items">A collection of items.</param>
public record YamlArray(IReadOnlyList<string> Items) : YamlValue
{
    /// <inheritdoc />
    public override int GetHashCode()
        => this.Items.Aggregate(0, HashCode.Combine);

    /// <inheritdoc />
    public virtual bool Equals(YamlArray? other)
        => other is not null && this.Items.SequenceEqual(other.Items);
}
