using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Application.GitHub;
using Keystone.Cli.Domain;
using Keystone.Cli.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Keystone.Cli.UnitTests.Application.Commands.New;

[TestFixture, Parallelizable(ParallelScope.All)]
public class NewCommandTests
{
    private static NewCommand Ctor(
        IGitHubService? gitHubService = null,
        ILogger<NewCommand>? logger = null,
        ITemplateService? templateService = null
    )
        => new(
            gitHubService ?? Substitute.For<IGitHubService>(),
            logger ?? Substitute.For<ILogger<NewCommand>>(),
            templateService ?? Substitute.For<ITemplateService>()
        );

    [Test]
    public async Task CreateNewAsync_ResolvesTemplateTargetAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";
        const string path = $"./{name}";

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Throws<KeyNotFoundException>();

        var sut = Ctor(templateService: templateService);

        await Assert.ThatAsync(
            () => sut.CreateNewAsync(name, templateName, path, CancellationToken.None),
            Throws.TypeOf<KeyNotFoundException>()
        );
    }

    [Test]
    public async Task CreateNewAsync_LogsProjectNameWithRepositoryUrlAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-a";
        const string path = $"./{name}";

        var templateTarget = new TemplateTargetModel(
            Name: templateName,
            RepositoryUrl: new Uri(repositoryUrl)
        );

        var logger = new TestLogger<NewCommand>();

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Returns(templateTarget);

        var sut = Ctor(logger: logger, templateService: templateService);
        await sut.CreateNewAsync(name, templateName, path, CancellationToken.None);

        Assert.That(
            logger.CapturedLogEntries,
            Has.Some.Matches<LogEntry>(entry => entry.Is(LogLevel.Information, $"Creating project '{name}' from {repositoryUrl}"))
        );
    }

    [Test]
    public async Task CreateNewAsync_CopiesPublicRepositoryAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-a";
        const string path = $"./{name}";

        var templateTarget = new TemplateTargetModel(
            Name: templateName,
            RepositoryUrl: new Uri(repositoryUrl)
        );

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Returns(templateTarget);

        var gitHubService = Substitute.For<IGitHubService>();

        var sut = Ctor(gitHubService, templateService: templateService);
        await sut.CreateNewAsync(name, templateName, path, CancellationToken.None);

        await gitHubService.Received(1).CopyPublicRepositoryAsync(
            templateTarget.RepositoryUrl,
            branchName: templateTarget.BranchName,
            destinationPath: path,
            overwrite: true,
            cancellationToken: CancellationToken.None
        );
    }
}
