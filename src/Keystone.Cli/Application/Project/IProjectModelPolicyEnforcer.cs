using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Project;

/// <summary>
/// Enforces policies related to the <see cref="ProjectModel"/>.
/// </summary>
public interface IProjectModelPolicyEnforcer
{
    /// <summary>
    /// Ensures the specified <see cref="ProjectModel"/> is considered loaded according to
    /// <see cref="ProjectModelLoadPolicy.IsLoaded(ProjectModel?)"/>. Throws a <see cref="ProjectNotLoadedException"/>
    /// if the model is not valid.
    /// </summary>
    /// <param name="projectModel">The model loaded from the specified path.</param>
    /// <exception cref="ProjectNotLoadedException">
    /// Thrown if the <paramref name="projectModel"/> does not satisfy
    /// <see cref="ProjectModelLoadPolicy.IsLoaded(ProjectModel?)"/>.
    /// </exception>
    void ThrowIfNotLoaded(ProjectModel projectModel);
}
