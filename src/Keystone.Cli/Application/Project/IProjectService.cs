using Keystone.Cli.Domain;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Project;

/// <summary>
/// Service for managing project settings and its metadata.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Creates a new project at the specified path using the given template.
    /// </summary>
    /// <param name="projectName">The new project name.</param>
    /// <param name="fullPathToProject">The full path to the new project directory.</param>
    /// <param name="templateTarget">The template for the new project.</param>
    /// <param name="includeGitFiles">Include Git-related files (e.g., <c>.gitattributes</c>, <c>.gitignore</c>) in the new project</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// The newly created <see cref="ProjectModel"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a project already exists at <paramref name="fullPathToProject"/>.
    /// </exception>
    Task<ProjectModel> CreateNewAsync(
        string projectName,
        string fullPathToProject,
        TemplateTargetModel templateTarget,
        bool includeGitFiles,
        CancellationToken cancellationToken
    );
}
