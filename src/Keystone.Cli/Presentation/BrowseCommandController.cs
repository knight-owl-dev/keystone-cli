using Cocona;
using JetBrains.Annotations;
using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Domain;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "browse" command controller.
/// </summary>
public class BrowseCommandController(IBrowseCommand browseCommand)
{
    [Command("browse", Description = "Opens the template repository in the default browser."), UsedImplicitly]
    public int Browse([Argument(Description = "The template name")] string? templateName)
    {
        try
        {
            browseCommand.Browse(templateName);

            return CliCommandResults.Success;
        }
        catch (KeyNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);

            return CliCommandResults.Error;
        }
    }
}
