using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.Project;
using Keystone.Cli.Domain;
using Keystone.Cli.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Commands.Project;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectCommandTests
{
    private static ProjectCommand Ctor(ILogger<ProjectCommand>? logger = null, ITemplateService? templateService = null)
        => new(
            logger ?? new TestLogger<ProjectCommand>(),
            templateService ?? Substitute.For<ITemplateService>()
        );

    [Test]
    public async Task SwitchTemplateAsync_NoChanges_ReturnsFalseAsync()
    {
        const string newTemplateName = "new-template";
        const string projectPath = "/path/to/project";

        var templateTarget = new TemplateTargetModel(
            Name: newTemplateName,
            RepositoryUrl: new Uri("https://github.com/knight-owl-dev/template-a")
        );

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(newTemplateName).Returns(templateTarget);

        var sut = Ctor(templateService: templateService);

        var actual = await sut.SwitchTemplateAsync(newTemplateName, projectPath, CancellationToken.None);

        Assert.That(actual, Is.False);
    }
}
