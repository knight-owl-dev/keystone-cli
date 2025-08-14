using System.ComponentModel.DataAnnotations;
using Cocona;
using Keystone.Cli.Application.Commands.Project;
using Keystone.Cli.Domain;
using Keystone.Cli.Presentation.ComponentModel.DataAnnotations;


namespace Keystone.Cli.Presentation.Project;

/// <summary>
/// The implementation of the "switch-template" sub-command for the project command.
/// </summary>
public class SwitchTemplateSubCommand(IProjectCommand projectCommand)
{
    public async Task<int> SwitchTemplateAsync(
        [Argument(Description = "The name of the new template to switch to"),
         NotPaddedWhitespace, Required(AllowEmptyStrings = false)]
        string newTemplateName,
        [Option(Description = "The path to the project where the template should be switched"),
         Path, NotPaddedWhitespace]
        string? projectPath = null,
        CancellationToken cancellationToken = default
    )
    {
        var fullPath = string.IsNullOrWhiteSpace(projectPath)
            ? Path.GetFullPath(".")
            : Path.GetFullPath(projectPath);

        Console.WriteLine($"Switching template to '{newTemplateName}' for project at '{fullPath}'.");
        try
        {
            var result = await projectCommand.SwitchTemplateAsync(newTemplateName, fullPath, cancellationToken);

            Console.WriteLine(
                result
                    ? $"Switched to template '{newTemplateName}' successfully."
                    : $"The project already uses template `{newTemplateName}`."
            );

            return CliCommandResults.Success;
        }
        catch (KeyNotFoundException exception)
        {
            await Console.Error.WriteLineAsync(exception.Message);

            return CliCommandResults.Error;
        }
    }
}
