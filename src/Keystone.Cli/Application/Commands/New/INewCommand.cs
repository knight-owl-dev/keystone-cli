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
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the template target is not found.
    /// </exception>
    void CreateNew(string name, string? templateName);
}
