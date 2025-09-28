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
        // TODO: read the original ProjectModel instead, check if it's already using the target template.
        var projectName = Path.GetFileName(projectPath);

        var templateTarget = templateService.GetTemplateTarget(newTemplateName);

        logger.LogInformation(
            "Switching project '{ProjectName}' to use {RepositoryUrl} template in {Path}",
            projectName,
            templateTarget.RepositoryUrl,
            projectPath
        );

        // TODO: similar to NewCommand, use IGitHubService.CopyPublicRepositoryAsync, must use EntryModelPolicies.ExcludeGitContent.
        // Then read the new project model (we need its sync data).
        // Update the original project model to use the new sync data and save it,
        // thus restoring the original project data while targeting the new template.

        return Task.FromResult(false);
    }
}
