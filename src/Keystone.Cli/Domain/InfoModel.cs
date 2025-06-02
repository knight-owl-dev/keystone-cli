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
);
