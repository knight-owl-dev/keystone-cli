using System.ComponentModel.DataAnnotations;
using Cocona;
using JetBrains.Annotations;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Domain;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Presentation.ComponentModel.DataAnnotations;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "new" command controller.
/// </summary>
public class NewCommandController(INewCommand newCommand)
{
    [Command("new", Description = "Creates a new project from a template."), UsedImplicitly]
    public async Task<int> NewAsync(
        [Argument(Description = "The name of the new project, also used as its root directory unless the project path is provided"),
         Required(AllowEmptyStrings = false), NotPaddedWhitespace]
        string projectName,
        [Option(Description = "The template name"),
         NotPaddedWhitespace]
        string? templateName = null,
        [Option(Description = "The path where to create the new project"),
         Path, NotPaddedWhitespace]
        string? projectPath = null
    )
    {
        var fullPath = string.IsNullOrWhiteSpace(projectPath)
            ? Path.Combine(Path.GetFullPath("."), ProjectNamePolicy.GetProjectDirectoryName(projectName))
            : Path.GetFullPath(projectPath);

        try
        {
            await newCommand.CreateNewAsync(
                projectName,
                templateName,
                fullPath,
                CancellationToken.None
            );

            return CliCommandResults.Success;
        }
        catch (KeyNotFoundException ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);

            return CliCommandResults.Error;
        }
    }
}
