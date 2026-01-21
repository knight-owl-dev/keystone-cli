using System.Text;


namespace Keystone.Cli.Application.Utility.Serialization.Yaml;

/// <summary>
/// The YAML scalar value type.
/// </summary>
/// <param name="Value">The scalar value.</param>
public record YamlScalar(string? Value) : YamlValue
{
    /// <summary>
    /// The YAML <c>null</c> value.
    /// </summary>
    public static readonly YamlScalar Null = new(Value: null);

    protected override bool PrintMembers(StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Append(this.Value);

        return true;
    }
}
