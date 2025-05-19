using Keystone.Cli.Domain;


namespace Keystone.Cli.Application.Data;

/// <summary>
/// The template targets repository.
/// </summary>
public interface ITemplateTargetsRepository
{
    /// <summary>
    /// Gets all supported template targets.
    /// </summary>
    /// <returns>
    /// A collection of template target models.
    /// </returns>
    IEnumerable<TemplateTargetModel> GetTemplateTargets();

    /// <summary>
    /// Gets the template target by its case-insensitive name.
    /// </summary>
    /// <param name="name">The case-insensitive name of the template target.</param>
    /// <returns>
    /// The resolved template target model, or null if not found.
    /// </returns>
    TemplateTargetModel? GetTemplateTarget(string name);
}
