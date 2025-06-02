namespace Keystone.Cli.Application.Commands.Browse;

/// <summary>
/// The "browse" command interface.
/// </summary>
public interface IBrowseCommand
{
    /// <summary>
    /// Executes the "browse" command.
    /// </summary>
    /// <param name="templateName">The target template name.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the template target is not found.
    /// </exception>
    void Browse(string? templateName);
}
