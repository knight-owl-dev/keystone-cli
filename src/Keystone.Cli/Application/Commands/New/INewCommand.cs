namespace Keystone.Cli.Application.Commands.New;

/// <summary>
/// The "new" command interface.
/// </summary>
public interface INewCommand
{
    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="name">The new project name, also used as its root directory.</param>
    /// <param name="templateName">The optional template name.</param>
    /// <param name="fullPathToProject">The full path to the new project directory.</param>
    /// <param name="includeGitFiles">Include Git-related files (e.g., <c>.gitattributes</c>, <c>.gitignore</c>) in the new project.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the project cannot be created. e.g., if there's already a project at the specified path.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the template target is not found.
    /// </exception>
    Task CreateNewAsync(
        string name,
        string? templateName,
        string fullPathToProject,
        bool includeGitFiles,
        CancellationToken cancellationToken
    );
}
