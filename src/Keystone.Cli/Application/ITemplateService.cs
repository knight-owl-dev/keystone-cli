using Keystone.Cli.Domain;


namespace Keystone.Cli.Application;

/// <summary>
/// The Keystone template service.
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Gets the template target by name.
    /// </summary>
    /// <remarks>
    /// If the <paramref name="name"/> is <c>null</c>, the default template target is returned.
    /// </remarks>
    /// <param name="name">The name of the template target; otherwise, <c>null</c> for the default value.</param>
    /// <returns>
    /// The template target model.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the template target is not found.
    /// </exception>
    TemplateTargetModel GetTemplateTarget(string? name);

    /// <summary>
    /// Gets all supported template targets.
    /// </summary>
    /// <returns>
    /// A collection of template target models.
    /// </returns>
    IEnumerable<TemplateTargetModel> GetTemplateTargets();
}
