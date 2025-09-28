using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.Project;
using Keystone.Cli.Application.Project;
using Keystone.Cli.Domain;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Commands.Project;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectCommandTests
{
    private static ProjectCommand Ctor(ITemplateService? templateService = null, IProjectService? projectService = null)
        => new(templateService ?? Substitute.For<ITemplateService>(), projectService ?? Substitute.For<IProjectService>());

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

        var sut = Ctor(templateService);

        var actual = await sut.SwitchTemplateAsync(newTemplateName, projectPath, CancellationToken.None);

        Assert.That(actual, Is.False);
    }

    [Test]
    public async Task SwitchTemplateAsync_HasChanges_ReturnsTrueAsync()
    {
        const string newTemplateName = "new-template";
        const string projectPath = "/path/to/project";

        var templateTarget = new TemplateTargetModel(
            Name: newTemplateName,
            RepositoryUrl: new Uri("https://github.com/knight-owl-dev/template-a")
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(newTemplateName).Returns(templateTarget);

        var projectService = Substitute.For<IProjectService>();
        projectService.SwitchTemplateAsync(projectPath, templateTarget, cancellationToken).Returns(true);

        var sut = Ctor(templateService, projectService);

        var actual = await sut.SwitchTemplateAsync(newTemplateName, projectPath, cancellationToken);

        Assert.That(actual, Is.True);
    }
}
