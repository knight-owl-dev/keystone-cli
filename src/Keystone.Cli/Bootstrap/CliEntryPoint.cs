using Cocona;
using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Application.Commands.Info;
using Keystone.Cli.Application.Commands.New;
using Microsoft.Extensions.DependencyInjection;


namespace Keystone.Cli.Bootstrap;

/// <summary>
/// The CLI entry point.
/// </summary>
public class CliEntryPoint(
    BrowseCommand browseCommand,
    InfoCommand infoCommand,
    NewCommand newCommand
)
{
    /// <summary>
    /// Bootstraps and runs the CLI application using Cocona.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public async Task RunAsync(string[] args)
    {
        var builder = CoconaApp.CreateBuilder(args);
        builder.Services.AddSingleton(browseCommand);
        builder.Services.AddSingleton(infoCommand);
        builder.Services.AddSingleton(newCommand);

        var app = builder.Build();
        app.AddCommands<InfoCommand>();
        app.AddCommands<BrowseCommand>();
        app.AddCommands<NewCommand>();

        await app.RunAsync();
    }
}
