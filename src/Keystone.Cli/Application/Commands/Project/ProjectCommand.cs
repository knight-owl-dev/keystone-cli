using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.Commands.Project;

/// <summary>
/// The default implementation of <see cref="IProjectCommand"/> for managing Keystone projects.
/// </summary>
public class ProjectCommand(ILogger<ProjectCommand> logger, ITemplateService templateService)
    : IProjectCommand
{
    /// <inheritdoc />
    public Task<bool> SwitchTemplateAsync(string newTemplateName, string projectPath, CancellationToken cancellationToken)
    {
        // TODO: read from the project metadata
        var projectName = Path.GetFileName(projectPath);

        var templateTarget = templateService.GetTemplateTarget(newTemplateName);

        logger.LogInformation(
            "Switching project '{ProjectName}' to use {RepositoryUrl} template in {Path}",
            projectName,
            templateTarget.RepositoryUrl,
            projectPath
        );

        return Task.FromResult(false);
    }
}
