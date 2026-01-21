using Keystone.Cli.Application.Utility;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.Commands.Browse;

/// <summary>
/// The "browse" command implementation.
/// </summary>
public partial class BrowseCommand(ILogger<BrowseCommand> logger, IProcessService processService, ITemplateService templateService)
    : IBrowseCommand
{
    /// <inheritdoc />
    public void Browse(string? templateName)
    {
        var templateTarget = templateService.GetTemplateTarget(templateName);

        LogOpeningRepository(logger, templateTarget.RepositoryUrl);
        processService.OpenBrowser(templateTarget.RepositoryUrl);
    }

    [LoggerMessage(LogLevel.Information, "Opening {RepositoryUrl}")]
    static partial void LogOpeningRepository(ILogger<BrowseCommand> logger, Uri repositoryUrl);
}
