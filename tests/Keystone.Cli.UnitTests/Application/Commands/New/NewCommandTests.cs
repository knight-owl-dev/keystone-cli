using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.New;
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
        ILogger<NewCommand>? logger = null,
        ITemplateService? templateService = null
    )
        => new(
            logger ?? Substitute.For<ILogger<NewCommand>>(),
            templateService ?? Substitute.For<ITemplateService>()
        );

    [Test]
    public void CreateNew_ResolvesTemplateTarget()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Throws<KeyNotFoundException>();

        var sut = Ctor(templateService: templateService);

        Assert.That(
            () => sut.CreateNew(name, templateName),
            Throws.TypeOf<KeyNotFoundException>()
        );
    }

    [Test]
    public void CreateNew_LogsProjectNameWithRepositoryUrl()
    {
        const string name = "project-name";
        const string templateName = "template-name";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-a";

        var templateTarget = new TemplateTargetModel(
            Name: templateName,
            RepositoryUrl: new Uri(repositoryUrl)
        );

        var logger = new TestLogger<NewCommand>();

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Returns(templateTarget);

        var sut = Ctor(logger, templateService);
        sut.CreateNew(name, templateName);

        Assert.That(
            logger.CapturedLogEntries,
            Has.Some.Matches<LogEntry>(entry => entry.Is(LogLevel.Information, $"Creating project '{name}' from {repositoryUrl}"))
        );
    }
}
