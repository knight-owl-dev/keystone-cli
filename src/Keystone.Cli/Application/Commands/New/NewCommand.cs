using Keystone.Cli.Application.GitHub;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.Commands.New;

/// <summary>
/// The "new" command handler.
/// </summary>
public class NewCommand(IGitHubService gitHubService, ILogger<NewCommand> logger, ITemplateService templateService)
    : INewCommand
{
    /// <inheritdoc />
    public async Task CreateNewAsync(string name, string? templateName, string fullPathToProject, CancellationToken cancellationToken)
    {
        var templateTarget = templateService.GetTemplateTarget(templateName);

        logger.LogInformation(
            "Creating project '{ProjectName}' from {RepositoryUrl} in {Path}",
            name,
            templateTarget.RepositoryUrl,
            fullPathToProject
        );

        await gitHubService.CopyPublicRepositoryAsync(
            templateTarget.RepositoryUrl,
            branchName: templateTarget.BranchName,
            destinationPath: fullPathToProject,
            overwrite: true,
            cancellationToken: cancellationToken
        );
    }
}
