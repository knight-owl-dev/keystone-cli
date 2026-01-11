using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Domain.Policies;

/// <summary>
/// Defines what it means to load a <see cref="ProjectModel"/> from a Keystone project.
/// </summary>
public static class ProjectModelLoadPolicy
{
    /// <summary>
    /// Evaluates whether a <see cref="ProjectModel"/> meets the minimum criteria to be considered loaded.
    /// </summary>
    /// <remarks>
    /// This policy may evolve to include additional checks as more fields become critical
    /// for project operations. Don't make any assumptions about completeness beyond the current checks.
    /// </remarks>
    /// <param name="projectModel">The project model instance to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the project model has a non-empty <see cref="ProjectModel.ProjectName"/>
    /// and a non-null <see cref="ProjectModel.KeystoneSync"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsLoaded(ProjectModel? projectModel)
        => projectModel is { ProjectName.Length: > 0, KeystoneSync: not null };
}
