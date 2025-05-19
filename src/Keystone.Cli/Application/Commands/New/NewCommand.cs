using Cocona;
using JetBrains.Annotations;


namespace Keystone.Cli.Application.Commands.New;

/// <summary>
/// The "new" command definition.
/// </summary>
[UsedImplicitly]
public class NewCommand(NewHandler handler)
{
    [Command("new", Description = "Creates a new project from a template."), UsedImplicitly]
    public void Execute(
        [Argument(Description = "The name of the new project, also used as its root directory")] string name,
        [Option(Description = "The template name")] string? templateName
    )
    {
        try
        {
            handler.CreateNew(name, templateName);
        }
        catch (KeyNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Environment.ExitCode = 1;
        }
    }
}
