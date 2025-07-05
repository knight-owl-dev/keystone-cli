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
    public static void AddDependencies(this IServiceCollection services)
        => services
            .AddSingleton<IBrowseCommand, BrowseCommand>()
            .AddSingleton<IInfoCommand, InfoCommand>()
            .AddSingleton<INewCommand, NewCommand>()
            .AddSingleton<IProcessService, ProcessService>()
            .AddSingleton<ITemplateService, TemplateService>()
            .AddSingleton<ITemplateTargetsRepository, TemplateTargetsRepository>();
}
