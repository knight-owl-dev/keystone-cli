using Keystone.Cli.Application.Project;


namespace Keystone.Cli.Application.Commands.New;

/// <summary>
/// The "new" command handler.
/// </summary>
public class NewCommand(ITemplateService templateService, IProjectService projectService)
    : INewCommand
{
    /// <inheritdoc />
    public async Task CreateNewAsync(
        string name,
        string? templateName,
        string fullPathToProject,
        bool includeGitFiles,
        CancellationToken cancellationToken
    )
    {
        var templateTarget = templateService.GetTemplateTarget(templateName);

        await projectService.CreateNewAsync(
            name,
            fullPathToProject,
            templateTarget,
            includeGitFiles,
            cancellationToken
        );
    }
}
