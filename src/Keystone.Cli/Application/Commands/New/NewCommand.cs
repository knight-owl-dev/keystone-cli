using Keystone.Cli.Application.GitHub;
using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.Domain.Policies;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.Commands.New;

/// <summary>
/// The "new" command handler.
/// </summary>
public class NewCommand(IGitHubService gitHubService, ILogger<NewCommand> logger, ITemplateService templateService)
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

        logger.LogInformation(
            "Creating project '{ProjectName}' from {RepositoryUrl} in {Path}",
            name,
            templateTarget.RepositoryUrl,
            fullPathToProject
        );

        Func<EntryModel, bool> predicate = includeGitFiles
            ? EntryModelPredicates.AcceptAll
            : EntryModelPolicies.ExcludeGitContent;

        await gitHubService.CopyPublicRepositoryAsync(
            templateTarget.RepositoryUrl,
            branchName: templateTarget.BranchName,
            destinationPath: fullPathToProject,
            overwrite: true,
            predicate: predicate,
            cancellationToken: cancellationToken
        );
    }
}
