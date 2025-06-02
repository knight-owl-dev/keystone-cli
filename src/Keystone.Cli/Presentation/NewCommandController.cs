using Cocona;
using JetBrains.Annotations;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Domain;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "new" command controller.
/// </summary>
public class NewCommandController(INewCommand newCommand)
{
    [Command("new", Description = "Creates a new project from a template."), UsedImplicitly]
    public int New(
        [Argument(Description = "The name of the new project, also used as its root directory")] string name,
        [Option(Description = "The template name")] string? templateName
    )
    {
        try
        {
            newCommand.CreateNew(name, templateName);

            return CliCommandResults.Success;
        }
        catch (KeyNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);

            return CliCommandResults.Error;
        }
    }
}
