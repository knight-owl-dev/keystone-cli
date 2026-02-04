using System.ComponentModel.DataAnnotations;
using Cocona;
using Cocona.Application;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Domain;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Presentation.ComponentModel.DataAnnotations;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "new" command controller.
/// </summary>
public class NewCommandController(
    ICoconaAppContextAccessor contextAccessor,
    IConsole console,
    INewCommand newCommand
)
{
    [Command("new", Description = "Creates a new project from a template")]
    public async Task<int> NewAsync(
        [Argument(Description = "The name of the new project, also used as its root directory unless the project path is provided"),
         Required(AllowEmptyStrings = false), NotPaddedWhitespace]
        string projectName,
        [Option('t', Description = "The template name"),
         NotPaddedWhitespace]
        string? templateName = null,
        [Option('p', Description = "The path where to create the new project"),
         Path, NotPaddedWhitespace]
        string? projectPath = null,
        [Option('g', Description = "Include Git-related files (e.g., .gitattributes, .gitignore) in the new project")]
        bool includeGitFiles = false
    )
    {
        var cancellationToken = contextAccessor.Current?.CancellationToken ?? CancellationToken.None;

        var fullPath = string.IsNullOrWhiteSpace(projectPath)
            ? Path.Combine(Path.GetFullPath("."), ProjectNamePolicy.GetProjectDirectoryName(projectName))
            : Path.GetFullPath(projectPath);

        try
        {
            await newCommand.CreateNewAsync(
                projectName,
                templateName,
                fullPath,
                includeGitFiles,
                cancellationToken
            );

            return CliCommandResults.Success;
        }
        catch (InvalidOperationException ex)
        {
            await console.Error.WriteLineAsync(ex.Message);

            return CliCommandResults.Error;
        }
        catch (KeyNotFoundException ex)
        {
            await console.Error.WriteLineAsync(ex.Message);

            return CliCommandResults.Error;
        }
    }
}
