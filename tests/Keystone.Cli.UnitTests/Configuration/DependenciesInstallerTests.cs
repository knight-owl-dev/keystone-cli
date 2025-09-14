using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Application.Commands.Info;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Application.Commands.Project;
using Keystone.Cli.Application.Data;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.GitHub;
using Keystone.Cli.Application.Project;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Keystone.Cli.UnitTests.Configuration;

[TestFixture, Parallelizable(ParallelScope.All)]
public class DependenciesInstallerTests
{
    private static readonly Type[] ExpectedTypes =
    [
        typeof(IBrowseCommand),
        typeof(IContentHashService),
        typeof(IFileSystemCopyService),
        typeof(IFileSystemService),
        typeof(IGitHubService),
        typeof(IGitHubZipEntryProviderFactory),
        typeof(IInfoCommand),
        typeof(INewCommand),
        typeof(IProcessService),
        typeof(IProjectCommand),
        typeof(IProjectModelPolicyEnforcer),
        typeof(ITemplateService),
        typeof(ITemplateTargetsRepository),
    ];

    [TestCaseSource(nameof(ExpectedTypes))]
    public void ResolvesExpectedTypes(Type type)
    {
        var settings = new Dictionary<string, string?>
        {
            ["Templates:core"] = "https://github.com/knight-owl-dev/keystone-template-core",
            ["Templates:core-slim"] = "https://github.com/knight-owl-dev/keystone-template-core-slim",
            ["DefaultTemplate"] = "core-slim",
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddDependencies();

        var serviceProvider = services.BuildServiceProvider();

        Assert.That(() => serviceProvider.GetService(type), Throws.Nothing);
    }
}
