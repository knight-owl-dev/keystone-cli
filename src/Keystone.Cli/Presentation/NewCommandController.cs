using System.ComponentModel.DataAnnotations;
using Cocona;
using JetBrains.Annotations;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Domain;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Presentation.CommandParameterSets;


namespace Keystone.Cli.Presentation;

/// <summary>
/// The "new" command controller.
/// </summary>
public class NewCommandController(INewCommand newCommand)
{
    [Command("new", Description = "Creates a new project from a template."), UsedImplicitly]
    public async Task<int> NewAsync(NewCommandParameterSet parameters)
    {
        try
        {
            parameters.Validate();

            var fullPath = string.IsNullOrWhiteSpace(parameters.ProjectPath)
                ? Path.Combine(Path.GetFullPath("."), ProjectNamePolicy.GetProjectDirectoryName(parameters.Name))
                : Path.GetFullPath(parameters.ProjectPath);

            await newCommand.CreateNewAsync(
                parameters.Name,
                parameters.TemplateName,
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
        catch (ValidationException ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);

            return CliCommandResults.ErrorInvalidArguments;
        }
    }
}
