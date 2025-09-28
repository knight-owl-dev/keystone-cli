using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Application.Project;
using Keystone.Cli.Domain;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Keystone.Cli.UnitTests.Application.Commands.New;

[TestFixture, Parallelizable(ParallelScope.All)]
public class NewCommandTests
{
    private static NewCommand Ctor(
        ITemplateService? templateService = null,
        IProjectService? projectService = null
    )
        => new(
            templateService ?? Substitute.For<ITemplateService>(),
            projectService ?? Substitute.For<IProjectService>()
        );

    [Test]
    public async Task CreateNewAsync_ResolvesTemplateTargetAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";
        const string path = $"./{name}";
        const bool includeGitFiles = false;

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Throws<KeyNotFoundException>();

        var sut = Ctor(templateService: templateService);

        await Assert.ThatAsync(
            () => sut.CreateNewAsync(name, templateName, path, includeGitFiles, CancellationToken.None),
            Throws.TypeOf<KeyNotFoundException>()
        );
    }

    [Test]
    public async Task CreateNewAsync_CallsProjectServiceCreateNewAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-a";
        const string path = $"./{name}";
        const bool includeGitFiles = false;

        var templateTarget = new TemplateTargetModel(
            Name: templateName,
            RepositoryUrl: new Uri(repositoryUrl)
        );

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Returns(templateTarget);

        var projectService = Substitute.For<IProjectService>();

        var sut = Ctor(templateService: templateService, projectService: projectService);
        await sut.CreateNewAsync(name, templateName, path, includeGitFiles, CancellationToken.None);

        await projectService.Received(1).CreateNewAsync(
            name,
            path,
            templateTarget,
            includeGitFiles,
            CancellationToken.None
        );
    }

    [Test]
    public async Task CreateNewAsync_WithGitContentExcluded_PassesCorrectIncludeGitFilesValueAsync()
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

        var projectService = Substitute.For<IProjectService>();

        var sut = Ctor(templateService: templateService, projectService: projectService);
        await sut.CreateNewAsync(name, templateName, path, includeGitFiles: false, CancellationToken.None);

        await projectService.Received(1).CreateNewAsync(
            name,
            path,
            templateTarget,
            includeGitFiles: false,
            CancellationToken.None
        );
    }

    [Test]
    public async Task CreateNewAsync_WithGitContentIncluded_PassesCorrectIncludeGitFilesValueAsync()
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

        var projectService = Substitute.For<IProjectService>();

        var sut = Ctor(templateService: templateService, projectService: projectService);
        await sut.CreateNewAsync(name, templateName, path, includeGitFiles: true, CancellationToken.None);

        await projectService.Received(1).CreateNewAsync(
            name,
            path,
            templateTarget,
            includeGitFiles: true,
            CancellationToken.None
        );
    }
}
