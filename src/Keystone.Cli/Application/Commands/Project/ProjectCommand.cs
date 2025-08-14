namespace Keystone.Cli.Application.Commands.Project;

/// <summary>
/// The default implementation of <see cref="IProjectCommand"/> for managing Keystone projects.
/// </summary>
public class ProjectCommand
    : IProjectCommand
{
    /// <inheritdoc />
    public Task<bool> SwitchTemplateAsync(string newTemplateName, string projectPath, CancellationToken cancellationToken)
        => Task.FromResult(false);
}
