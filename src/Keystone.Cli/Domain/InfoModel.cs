using System.Text;


namespace Keystone.Cli.Domain;

/// <summary>
/// The program information model.
/// </summary>
/// <param name="Version">The current version.</param>
/// <param name="Description">The program description.</param>
/// <param name="Copyright">The program copyright string.</param>
/// <param name="DefaultTemplateTarget">The default template target.</param>
/// <param name="TemplateTargets">All available template targets.</param>
public record InfoModel(
    string? Version,
    string? Description,
    string? Copyright,
    TemplateTargetModel DefaultTemplateTarget,
    IReadOnlyList<TemplateTargetModel> TemplateTargets
)
{
    /// <summary>
    /// Gets the formatted text representation of the info model.
    /// </summary>
    /// <returns>
    /// The formated test representation of the info model.
    /// </returns>
    public string GetFormattedText()
    {
        var formatProvider = System.Globalization.CultureInfo.InvariantCulture;

        var version = this.Version ?? "unknown";
        var description = this.Description ?? "No description available.";
        var copyright = this.Copyright ?? "No copyright information available.";
        var maxTemplateNameLength = GetMaxTemplateNameLength();

        var buffer = new StringBuilder();

        buffer.AppendLine(formatProvider, $"Keystone CLI v{version}. {copyright}");
        buffer.AppendLine(formatProvider, $"{description}");
        buffer.AppendLine();

        buffer.AppendLine("Available Keystone template targets:");
        foreach (var (name, repositoryUrl, _) in this.TemplateTargets)
        {
            buffer.AppendLine(formatProvider, $" - {name.PadLeft(maxTemplateNameLength, ' ')}: {repositoryUrl}");
        }

        buffer.AppendLine();
        buffer.AppendLine(formatProvider, $"Default template: {this.DefaultTemplateTarget.Name}");

        return buffer.ToString();
    }

    private int GetMaxTemplateNameLength()
        => this.TemplateTargets.Count > 0 ? this.TemplateTargets.Max(target => target.Name.Length) : 0;
}
