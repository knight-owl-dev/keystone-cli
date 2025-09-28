namespace Keystone.Cli.Application.Project;

/// <summary>
/// Service for managing project settings and its metadata.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Sets the name of the project.
    /// </summary>
    /// <param name="projectRoot">The root directory of the project.</param>
    /// <param name="name">The new name of the project.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task SetProjectNameAsync(string projectRoot, string name, CancellationToken cancellationToken);
}
