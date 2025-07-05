using Cocona;
using JetBrains.Annotations;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Domain;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "new" command controller.
/// </summary>
public class NewCommandController(INewCommand newCommand)
{
    [Command("new", Description = "Creates a new project from a template."), UsedImplicitly]
    public async Task<int> NewAsync(
        [Argument(Description = "The name of the new project, also used as its root directory unless the full path is provided")] string name,
        [Option(Description = "The template name")] string? templateName,
        [Option(Description = "The full path where to create the new project")] string? path
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var fullPath = string.IsNullOrWhiteSpace(path)
            ? Path.Combine(Path.GetFullPath("."), ProjectNamePolicy.GetProjectDirectoryName(name))
            : Path.GetFullPath(path);

        try
        {
            await newCommand.CreateNewAsync(
                name,
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
