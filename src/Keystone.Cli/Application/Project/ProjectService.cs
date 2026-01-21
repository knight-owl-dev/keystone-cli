using Keystone.Cli.Application.Data;
using Keystone.Cli.Application.GitHub;
using Keystone.Cli.Domain;
using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Domain.Project;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.Project;

/// <summary>
/// Default implementation of <see cref="IProjectService"/> that manages project settings and metadata.
/// </summary>
public partial class ProjectService(
    IGitHubService gitHubService,
    ILogger<ProjectService> logger,
    IProjectModelRepository projectModelRepository
)
    : IProjectService
{
    /// <inheritdoc />
    public async Task<ProjectModel> CreateNewAsync(
        string projectName,
        string fullPathToProject,
        TemplateTargetModel templateTarget,
        bool includeGitFiles,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentException.ThrowIfNullOrEmpty(fullPathToProject);
        ArgumentNullException.ThrowIfNull(templateTarget);

        LogCreatingProject(logger, projectName, templateTarget.RepositoryUrl, fullPathToProject);

        try
        {
            var existingProject = await projectModelRepository.LoadAsync(fullPathToProject, cancellationToken);

            throw new InvalidOperationException($"A project already exists at '{fullPathToProject}' with name '{existingProject.ProjectName}'.");
        }
        catch (ProjectNotLoadedException)
        {
            // we actually want to create a new project, so ignore this exception
        }

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

        var projectModel = await projectModelRepository.LoadAsync(fullPathToProject, cancellationToken) with
        {
            ProjectName = projectName,
        };

        await projectModelRepository.SaveAsync(projectModel, cancellationToken);

        return projectModel;
    }

    /// <inheritdoc />
    public async Task<bool> SwitchTemplateAsync(string fullPathToProject, TemplateTargetModel templateTarget, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(fullPathToProject);
        ArgumentNullException.ThrowIfNull(templateTarget);

        var originalProjectModel = await projectModelRepository.LoadAsync(fullPathToProject, cancellationToken);

        if (originalProjectModel.KeystoneSync?.TemplateRepositoryName == templateTarget.Name)
        {
            LogProjectAlreadyUsesRepository(logger, originalProjectModel.ProjectName!, templateTarget.RepositoryUrl);

            return false;
        }

        LogSwitchingProject(logger, originalProjectModel.ProjectName!, templateTarget.RepositoryUrl, fullPathToProject);

        await gitHubService.CopyPublicRepositoryAsync(
            templateTarget.RepositoryUrl,
            branchName: templateTarget.BranchName,
            destinationPath: fullPathToProject,
            overwrite: true,
            predicate: EntryModelPolicies.ExcludeGitAndUserContent,
            cancellationToken: cancellationToken
        );

        // reload the project model to pick up the KeystoneSync changes from the new template
        var refreshedProjectModel = await projectModelRepository.LoadAsync(fullPathToProject, cancellationToken);

        // preserve all properties except KeystoneSync, which we want to update
        var updatedProjectModel = originalProjectModel with
        {
            KeystoneSync = refreshedProjectModel.KeystoneSync,
        };

        await projectModelRepository.SaveAsync(updatedProjectModel, cancellationToken);

        return true;
    }

    [LoggerMessage(LogLevel.Information, "Creating project '{ProjectName}' from {RepositoryUrl} in {Path}")]
    static partial void LogCreatingProject(ILogger<ProjectService> logger, string projectName, Uri repositoryUrl, string path);

    [LoggerMessage(LogLevel.Information, "Project '{ProjectName}' already uses {RepositoryUrl} template, no change made")]
    static partial void LogProjectAlreadyUsesRepository(ILogger<ProjectService> logger, string projectName, Uri repositoryUrl);

    [LoggerMessage(LogLevel.Information, "Switching project '{ProjectName}' to use {RepositoryUrl} template in {Path}")]
    static partial void LogSwitchingProject(ILogger<ProjectService> logger, string projectName, Uri repositoryUrl, string path);
}
