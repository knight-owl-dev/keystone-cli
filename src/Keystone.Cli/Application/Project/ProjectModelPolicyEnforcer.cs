using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Project;

/// <summary>
/// The default implementation of <see cref="IProjectModelPolicyEnforcer"/>.
/// </summary>
public class ProjectModelPolicyEnforcer
    : IProjectModelPolicyEnforcer
{
    /// <inheritdoc />
    public void ThrowIfNotLoaded(ProjectModel projectModel)
    {
        ArgumentNullException.ThrowIfNull(projectModel);

        if (ProjectModelLoadPolicy.IsLoaded(projectModel))
        {
            return;
        }

        throw new ProjectNotLoadedException(
            $"Failed to load the Keystone project at '{projectModel.ProjectPath}'. "
            + "Ensure the project was initialized correctly and all necessary files are present."
        );
    }
}
