using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.Commands.New;

/// <summary>
/// The "new" command handler.
/// </summary>
public class NewCommand(ILogger<NewCommand> logger, ITemplateService templateService)
    : INewCommand
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

        logger.LogInformation("Creating project '{ProjectName}' from {RepositoryUrl}", name, templateTarget.RepositoryUrl);

        // TODO: clone from appropriate GitHub repo
    }
}
