using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Data;

/// <summary>
/// Represents a source of partial <see cref="ProjectModel"/> data loaded from or saved to
/// a specific medium in a Keystone project.
/// </summary>
/// <remarks>
/// Each implementation is responsible for reading and writing a subset of <see cref="ProjectModel"/> data
/// from a specific file, such as <c>.env</c>, <c>pandoc.yaml</c>, <c>sync.json</c>, or <c>publish.txt</c>.
/// </remarks>
public interface IProjectModelStore
{
    /// <summary>
    /// Loads the portion of the project model defined in the associated medium.
    /// </summary>
    /// <param name="model">The <see cref="ProjectModel"/> that may be empty or partially loaded.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A copy of the original <see cref="ProjectModel"/> instance with only the fields relevant
    /// to this store updated.
    /// </returns>
    Task<ProjectModel> LoadAsync(ProjectModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists the relevant portion of the <see cref="ProjectModel"/> to the associated medium.
    /// </summary>
    /// <param name="model">The <see cref="ProjectModel"/> to save.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates a stable hash of the portion of the project model managed by this store.
    /// Used to detect changes and avoid unnecessary writes.
    /// </summary>
    /// <param name="model">The project model to hash.</param>
    /// <returns>A string hash representing the store’s portion of the model.</returns>
    string GetContentHash(ProjectModel model);
}
