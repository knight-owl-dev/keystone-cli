using Cocona;
using JetBrains.Annotations;


namespace Keystone.Cli.Application.Commands.Info;

/// <summary>
/// The "info" command controller.
/// </summary>
public class InfoCommandController(IInfoCommand infoCommand)
{
    [Command("info", Description = "Prints the template information."), UsedImplicitly]
    public void Info()
        => infoCommand.PrintInfo();
}
