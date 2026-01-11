using Keystone.Cli.Application.Project;


namespace Keystone.Cli.Application.Commands.Project;

/// <summary>
/// The default implementation of <see cref="IProjectCommand"/> for managing Keystone projects.
/// </summary>
public class ProjectCommand(ITemplateService templateService, IProjectService projectService)
    : IProjectCommand
{
    /// <inheritdoc />
    public Task<bool> SwitchTemplateAsync(string newTemplateName, string projectPath, CancellationToken cancellationToken)
    {
        var templateTarget = templateService.GetTemplateTarget(newTemplateName);

        return projectService.SwitchTemplateAsync(projectPath, templateTarget, cancellationToken);
    }
}
