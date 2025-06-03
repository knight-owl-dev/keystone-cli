using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Domain;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Commands.Browse;

[TestFixture, Parallelizable(ParallelScope.All)]
public class BrowseCommandTests
{
    private static BrowseCommand Ctor(
        IProcessService? processService = null,
        ITemplateService? templateService = null
    )
        => new(
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
        var sut = Ctor(processService, templateService);

        sut.Browse(templateName);

        processService.Received(1).OpenBrowser(templateTarget.RepositoryUrl);
    }
}
