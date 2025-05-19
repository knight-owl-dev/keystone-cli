using Cocona;
using JetBrains.Annotations;


namespace Keystone.Cli.Application.Commands.Info;

/// <summary>
/// The "info" command definition.
/// </summary>
[UsedImplicitly]
public class InfoCommand(InfoHandler handler)
{
    [Command("info", Description = "Prints the template information."), UsedImplicitly]
    public void Execute()
        => handler.PrintInfo();
}
