using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Commands.Project;

/// <summary>
/// Defines operations for managing a Keystone project.
/// </summary>
public interface IProjectCommand
{
    /// <summary>
    /// Switches the template used by the current project.
    /// </summary>
    /// <remarks>
    /// If the specified template is already in use, no changes are made.
    /// </remarks>
    /// <param name="newTemplateName">The name of the new template to use.</param>
    /// <param name="projectPath">The full path to the project directory.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// <c>true</c> if the template was updated; otherwise, <c>false</c> if no changes were necessary.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified template does not exist in the Keystone templates registry.
    /// </exception>
    /// <exception cref="ProjectNotLoadedException">
    /// Thrown if the project at the specified path could not be loaded.
    /// </exception>
    Task<bool> SwitchTemplateAsync(string newTemplateName, string projectPath, CancellationToken cancellationToken);
}
