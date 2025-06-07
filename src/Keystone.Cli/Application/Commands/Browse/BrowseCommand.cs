using Keystone.Cli.Application.Utility;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.Commands.Browse;

/// <summary>
/// The "browse" command implementation.
/// </summary>
public class BrowseCommand(ILogger<BrowseCommand> logger, IProcessService processService, ITemplateService templateService)
    : IBrowseCommand
{
    /// <inheritdoc />
    public void Browse(string? templateName)
    {
        var templateTarget = templateService.GetTemplateTarget(templateName);

        logger.LogInformation("Opening {RepositoryUrl}", templateTarget.RepositoryUrl);
        processService.OpenBrowser(templateTarget.RepositoryUrl);
    }
}
