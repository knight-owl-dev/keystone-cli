using Cocona;
using Keystone.Cli.Application.Commands.Info;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "info" command controller.
/// </summary>
public class InfoCommandController(IInfoCommand infoCommand)
{
    [Command("info", Description = "Prints the template information")]
    public void Info()
    {
        var info = infoCommand.GetInfo();
        var text = info.GetFormattedText();

        Console.WriteLine(text);
    }
}
