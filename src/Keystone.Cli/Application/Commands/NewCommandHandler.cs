using JetBrains.Annotations;


namespace Keystone.Cli.Application.Commands;

/// <summary>
/// The "new" command handler.
/// </summary>
[UsedImplicitly]
public class NewCommandHandler(ITemplateService templateService)
{
    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="name">The new project name, also used as its root directory.</param>
    /// <param name="templateName">The optional template name.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the template target is not found.
    /// </exception>
    public void CreateNew(string name, string? templateName)
    {
        var templateTarget = templateService.GetTemplateTarget(templateName);

        Console.WriteLine($"Creating project '{name}' from {templateTarget.RepositoryUrl}");

        // TODO: clone from appropriate GitHub repo
    }
}
