using Cocona;
using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Domain;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "browse" command controller.
/// </summary>
public class BrowseCommandController(IConsole console, IBrowseCommand browseCommand)
{
    [Command("browse", Description = "Opens the template repository in the default browser")]
    public int Browse([Argument(Description = "The template name")] string? templateName)
    {
        try
        {
            browseCommand.Browse(templateName);

            return CliCommandResults.Success;
        }
        catch (KeyNotFoundException ex)
        {
            console.Error.WriteLine(ex.Message);

            return CliCommandResults.Error;
        }
    }
}
