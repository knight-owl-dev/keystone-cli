using System.Text;
using Cocona;
using JetBrains.Annotations;
using Keystone.Cli.Application.Commands.Info;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "info" command controller.
/// </summary>
public class InfoCommandController(IInfoCommand infoCommand)
{
    [Command("info", Description = "Prints the template information."), UsedImplicitly]
    public void Info()
    {
        var info = infoCommand.GetInfo();

        var version = info.Version ?? "unknown";
        var description = info.Description ?? "No description available.";
        var copyright = info.Copyright ?? "No copyright information available.";

        var buffer = new StringBuilder();

        buffer.AppendLine($"Keystone CLI v{version}. {copyright}");
        buffer.AppendLine($"{description}");
        buffer.AppendLine();

        buffer.AppendLine("Available Keystone template targets:");
        foreach (var (name, repositoryUrl) in info.TemplateTargets)
        {
            buffer.AppendLine($" - {name,10}: {repositoryUrl}");
        }

        buffer.AppendLine();
        buffer.AppendLine($"Default template: {info.DefaultTemplateTarget.Name}");

        Console.Write(buffer);
    }
}
