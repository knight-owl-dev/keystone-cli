using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Application.Commands.Info;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Application.Data;
using Keystone.Cli.Application.Utility;
using Microsoft.Extensions.DependencyInjection;


namespace Keystone.Cli.Configuration;

/// <summary>
/// Installs the services for IoC.
/// </summary>
public static class DependenciesInstaller
{
    /// <summary>
    /// Adds the services to the service collection.
    /// </summary>
    /// <param name="services">The destination services collection to update.</param>
    /// <returns>
    /// The original collection with the services added.
    /// </returns>
    public static IServiceCollection AddDependencies(this IServiceCollection services)
        => services
            .AddSingleton<ITemplateService, TemplateService>()
            .AddSingleton<ITemplateTargetsRepository, TemplateTargetsRepository>()
            .AddSingleton<IProcessService, ProcessService>();

    /// <summary>
    /// Adds the commands to the service collection.
    /// </summary>
    /// <param name="services">Teh destination services collection to update.</param>
    /// <returns>
    /// The original collection with the commands added.
    /// </returns>
    public static IServiceCollection AddCommands(this IServiceCollection services)
        => services
            .AddSingleton<BrowseCommand>()
            .AddSingleton<BrowseHandler>()
            .AddSingleton<InfoCommand>()
            .AddSingleton<InfoHandler>()
            .AddSingleton<NewCommand>()
            .AddSingleton<NewHandler>();
}
