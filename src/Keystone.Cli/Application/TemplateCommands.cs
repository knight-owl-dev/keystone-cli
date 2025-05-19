using Cocona;
using JetBrains.Annotations;
using Keystone.Cli.Application.Commands;


namespace Keystone.Cli.Application;

/// <summary>
/// CLI commands for using Keystone templates.
/// </summary>
/// <remarks>
/// This is the command "router" or "controller" for the CLI based on the Cocona framework.
/// </remarks>
public class TemplateCommands(
    BrowseCommandHandler browseCommandHandler,
    InfoCommandHandler infoCommandHandler,
    NewCommandHandler newCommandHandler
)
{
    [Command("info", Description = "Prints the template information."), UsedImplicitly]
    public void Info()
        => infoCommandHandler.PrintInfo();

    [Command("browse", Description = "Opens the template repository in the default browser."), UsedImplicitly]
    public void Browse([Argument(Description = "The template name")] string? templateName)
    {
        try
        {
            browseCommandHandler.Browse(templateName);
        }
        catch (KeyNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Environment.ExitCode = 1;
        }
    }

    [Command("new", Description = "Creates a new project from a template."), UsedImplicitly]
    public void New(
        [Argument(Description = "The name of the new project, also used as its root directory")] string name,
        [Option(Description = "The template name")] string? templateName
    )
    {
        try
        {
            newCommandHandler.CreateNew(name, templateName);
        }
        catch (KeyNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
