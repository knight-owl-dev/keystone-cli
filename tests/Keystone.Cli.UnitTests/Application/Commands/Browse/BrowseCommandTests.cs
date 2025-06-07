using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Domain;
using Keystone.Cli.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Commands.Browse;

[TestFixture, Parallelizable(ParallelScope.All)]
public class BrowseCommandTests
{
    private static BrowseCommand Ctor(
        ILogger<BrowseCommand>? logger = null,
        IProcessService? processService = null,
        ITemplateService? templateService = null
    )
        => new(
            logger ?? Substitute.For<ILogger<BrowseCommand>>(),
            processService ?? Substitute.For<IProcessService>(),
            templateService ?? Substitute.For<ITemplateService>()
        );

    [Test]
    public void Browse_OpensBrowserWithTemplateRepositoryUrl()
    {
        const string templateName = "template-a";

        var templateTarget = new TemplateTargetModel(
            Name: templateName,
            RepositoryUrl: new Uri("https://github.com/knight-owl-dev/template-a")
        );

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Returns(templateTarget);

        var processService = Substitute.For<IProcessService>();
        var sut = Ctor(processService: processService, templateService: templateService);

        sut.Browse(templateName);

        processService.Received(1).OpenBrowser(templateTarget.RepositoryUrl);
    }

    [Test]
    public void Browse_LogsRepositoryUrl()
    {
        const string templateName = "template-a";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-a";

        var templateTarget = new TemplateTargetModel(
            Name: templateName,
            RepositoryUrl: new Uri(repositoryUrl)
        );

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Returns(templateTarget);

        var logger = new TestLogger<BrowseCommand>();
        var sut = Ctor(logger, templateService: templateService);

        sut.Browse(templateName);

        Assert.That(
            logger.CapturedLogEntries,
            Has.Some.Matches<LogEntry>(entry => entry.Is(LogLevel.Information, repositoryUrl))
        );
    }
}
