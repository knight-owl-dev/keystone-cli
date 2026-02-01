using System.Reflection;
using Keystone.Cli.Domain;


namespace Keystone.Cli.Application.Commands.Info;

/// <summary>
/// The "info" command implementation.
/// </summary>
public class InfoCommand(ITemplateService templateService)
    : IInfoCommand
{
    /// <inheritdoc />
    public InfoModel GetInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();

        return new InfoModel(
            Version: assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
            Description: assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
            Copyright: assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright,
            DefaultTemplateTarget: templateService.GetTemplateTarget(name: null),
            TemplateTargets: [.. templateService.GetTemplateTargets()]
        );
    }
}
