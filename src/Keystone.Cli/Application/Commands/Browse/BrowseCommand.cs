using Cocona;
using JetBrains.Annotations;


namespace Keystone.Cli.Application.Commands.Browse;

/// <summary>
/// The "browse" command definition.
/// </summary>
[UsedImplicitly]
public class BrowseCommand(BrowseHandler handler)
{
    [Command("browse", Description = "Browse the template repository."), UsedImplicitly]
    public void Execute([Argument(Description = "The template name")] string? templateName)
    {
        try
        {
            handler.Browse(templateName);
        }
        catch (KeyNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
