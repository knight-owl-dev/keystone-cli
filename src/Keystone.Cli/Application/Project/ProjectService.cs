using Keystone.Cli.Application.Data;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.Project;

/// <summary>
/// Default implementation of <see cref="IProjectService"/> that manages project settings and metadata.
/// </summary>
public class ProjectService(ILogger<ProjectService> logger, IProjectModelRepository projectModelRepository) : IProjectService
{
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
