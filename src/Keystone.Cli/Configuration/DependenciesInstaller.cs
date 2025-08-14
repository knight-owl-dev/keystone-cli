using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Application.Commands.Info;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Application.Commands.Project;
using Keystone.Cli.Application.Data;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.GitHub;
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
            .AddHttpClient()
            .AddSingleton<IBrowseCommand, BrowseCommand>()
            .AddSingleton<IFileSystemCopyService, FileSystemCopyService>()
            .AddSingleton<IFileSystemService, FileSystemService>()
            .AddSingleton<IGitHubService, GitHubService>()
            .AddSingleton<IGitHubZipEntryProviderFactory, GitHubZipEntryProviderFactory>()
            .AddSingleton<IInfoCommand, InfoCommand>()
            .AddSingleton<INewCommand, NewCommand>()
            .AddSingleton<IProcessService, ProcessService>()
            .AddSingleton<IProjectCommand, ProjectCommand>()
            .AddSingleton<ITemplateService, TemplateService>()
            .AddSingleton<ITemplateTargetsRepository, TemplateTargetsRepository>();
}
