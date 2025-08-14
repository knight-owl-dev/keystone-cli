using Cocona;


namespace Keystone.Cli.Presentation.Project;

/// <summary>
/// Defines the entry point for project-related sub-commands.
/// </summary>
/// <remarks>
/// Use the <see cref="HasSubCommandsAttribute"/> to register sub-command controllers
/// under this group. Enables syntax like:
/// <c>./keystone-cli project switch-template</c>
/// </remarks>
[HasSubCommands(typeof(SwitchTemplateSubCommand), "switch-template", Description = "Switches the template of the current project")]
public class ProjectCommandController;
