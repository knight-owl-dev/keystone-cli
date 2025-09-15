using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Data;

/// <summary>
/// Provides access to the full <see cref="ProjectModel"/> for a Keystone project by coordinating multiple stores.
/// </summary>
/// <remarks>
/// This repository loads the complete <see cref="ProjectModel"/> by aggregating data from all registered
/// <see cref="IProjectModelStore"/> instances. Saving writes back only the relevant portions to each store.
/// </remarks>
public interface IProjectModelRepository
{
    /// <summary>
    /// Loads the full <see cref="ProjectModel"/> from the given Keystone project path.
    /// </summary>
    /// <param name="projectPath">The root path of the Keystone project.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A fully populated <see cref="ProjectModel"/> assembled from all known sources.</returns>
    /// <exception cref="ProjectNotLoadedException">
    /// Thrown if the project is not properly initialized or required files are missing.
    /// </exception>
    Task<ProjectModel> LoadAsync(string projectPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the current <see cref="ProjectModel"/> to all associated backing stores.
    /// </summary>
    /// <param name="model">The <see cref="ProjectModel"/> to persist.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default);
}
