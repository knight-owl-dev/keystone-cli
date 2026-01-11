using Cocona;
using Keystone.Cli.Application.Commands.Info;
using Keystone.Cli.Application.Utility;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "info" command controller.
/// </summary>
public class InfoCommandController(IConsole console, IInfoCommand infoCommand)
{
    [Command("info", Description = "Prints the template information")]
    public void Info()
    {
        var info = infoCommand.GetInfo();
        var text = info.GetFormattedText();

        console.Out.WriteLine(text);
    }
}
