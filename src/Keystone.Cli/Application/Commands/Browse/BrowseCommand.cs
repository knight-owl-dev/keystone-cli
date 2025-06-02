using Keystone.Cli.Application.Utility;


namespace Keystone.Cli.Application.Commands.Browse;

/// <summary>
/// The "browse" command implementation.
/// </summary>
public class BrowseCommand(IProcessService processService, ITemplateService templateService)
    : IBrowseCommand
{
    /// <inheritdoc />
    public void Browse(string? templateName)
    {
        var templateTarget = templateService.GetTemplateTarget(templateName);

        Console.WriteLine($"Opening {templateTarget.RepositoryUrl}");
        processService.OpenBrowser(templateTarget.RepositoryUrl);
    }
}
