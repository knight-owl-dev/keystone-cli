using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands;
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
            .AddSingleton<ITemplateService, TemplateService>()
            .AddSingleton<ITemplateTargetsRepository, TemplateTargetsRepository>()
            .AddSingleton<IProcessService, ProcessService>()
            .AddSingleton<BrowseCommandHandler>()
            .AddSingleton<InfoCommandHandler>()
            .AddSingleton<NewCommandHandler>();
}
