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
public class ProjectService(IGitHubService gitHubService, ILogger<ProjectService> logger, IProjectModelRepository projectModelRepository) : IProjectService
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
        logger.LogInformation(
            "Creating project '{ProjectName}' from {RepositoryUrl} in {Path}",
            projectName,
            templateTarget.RepositoryUrl,
            fullPathToProject
        );

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
    public async Task SetProjectNameAsync(string projectRoot, string name, CancellationToken cancellationToken)
    {
        var projectModel = await projectModelRepository.LoadAsync(projectRoot, cancellationToken);

        var updatedModel = projectModel with
        {
            ProjectName = name,
        };

        await projectModelRepository.SaveAsync(updatedModel, cancellationToken);

        logger.LogInformation(
            "Updated the project name from '{OldProjectName}' to '{NewProjectName}' at '{ProjectPath}'",
            projectModel.ProjectName,
            name,
            projectRoot
        );
    }
}
