using System.Reflection;
using JetBrains.Annotations;


namespace Keystone.Cli.Application.Commands.Info;

/// <summary>
/// The "info" command handler.
/// </summary>
[UsedImplicitly]
public class InfoHandler(ITemplateService templateService)
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    public void PrintInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
        var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "unknown";
        var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "unknown";
        var defaultTemplateTarget = templateService.GetTemplateTarget(name: null);

        Console.WriteLine($"Keystone CLI v{version}. {copyright}");
        Console.WriteLine($"{description}");
        Console.WriteLine();

        Console.WriteLine("Available Keystone template targets:");
        foreach (var (name, repositoryUrl) in templateService.GetTemplateTargets())
        {
            Console.WriteLine($" - {name,10}: {repositoryUrl}");
        }

        Console.WriteLine();
        Console.WriteLine($"Default template: {defaultTemplateTarget.Name}");
    }
}
