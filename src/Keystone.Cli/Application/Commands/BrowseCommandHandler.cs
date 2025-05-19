using JetBrains.Annotations;
using Keystone.Cli.Application.Utility;


namespace Keystone.Cli.Application.Commands;

/// <summary>
/// The "browse" command handler.
/// </summary>
[UsedImplicitly]
public class BrowseCommandHandler(IProcessService processService, ITemplateService templateService)
{
    /// <summary>
    /// Executes the "browse" command.
    /// </summary>
    /// <param name="templateName">The target template name.</param>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the template target is not found.
    /// </exception>
    public void Browse(string? templateName)
    {
        var templateTarget = templateService.GetTemplateTarget(templateName);

        Console.WriteLine($"Opening {templateTarget.RepositoryUrl}...");
        processService.OpenBrowser(templateTarget.RepositoryUrl);
    }
}
