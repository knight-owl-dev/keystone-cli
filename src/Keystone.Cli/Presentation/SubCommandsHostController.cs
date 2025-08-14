using Cocona;
using Keystone.Cli.Presentation.Project;


namespace Keystone.Cli.Presentation;

/// <summary>
/// Serves as the entry point for top-level commands that organize sub-commands.
/// </summary>
/// <remarks>
/// Use the <see cref="HasSubCommandsAttribute"/> to register controllers with sub-commands.
/// <para>
/// For example, this enables syntax like:
/// <c>./keystone-cli project switch-template</c>
/// </para>
/// </remarks>
[HasSubCommands(typeof(ProjectCommandController), "project", Description = "Manages projects, including switching templates")]
public class SubCommandsHostController;
