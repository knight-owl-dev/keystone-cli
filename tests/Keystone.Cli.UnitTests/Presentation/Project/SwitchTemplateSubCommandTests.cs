using Keystone.Cli.Application.Commands.Project;
using Keystone.Cli.Domain;
using Keystone.Cli.Domain.Project;
using Keystone.Cli.Presentation.Project;
using Keystone.Cli.UnitTests.Application.Utility;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Keystone.Cli.UnitTests.Presentation.Project;

[TestFixture, Parallelizable(ParallelScope.All)]
public class SwitchTemplateSubCommandTests
{
    private static SwitchTemplateSubCommand Ctor(IProjectCommand? projectCommand = null)
        => new(NullConsole.Instance, projectCommand ?? Substitute.For<IProjectCommand>());

    [Test]
    public async Task SwitchTemplateAsync_OnSuccess_ReturnsCliSuccessAsync()
    {
        const string newTemplateName = "new-template";
        const string projectPath = ".";

        var projectCommand = Substitute.For<IProjectCommand>();

        projectCommand
            .SwitchTemplateAsync(newTemplateName, projectPath, Arg.Any<CancellationToken>())
            .Returns(true);

        var sut = Ctor(projectCommand);
        var actual = await sut.SwitchTemplateAsync(newTemplateName, projectPath);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Success));
    }

    [Test]
    public async Task SwitchTemplateAsync_TemplateNotFound_ReturnsCliFailureAsync()
    {
        const string newTemplateName = "non-existent-template";
        const string projectPath = ".";

        var projectCommand = Substitute.For<IProjectCommand>();

        projectCommand
            .SwitchTemplateAsync(newTemplateName, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new KeyNotFoundException($"Template '{newTemplateName}' not found."));

        var sut = Ctor(projectCommand);
        var actual = await sut.SwitchTemplateAsync(newTemplateName, projectPath);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Error));
    }

    [Test]
    public async Task SwitchTemplateAsync_ProjectNotLoaded_ReturnsCliFailureAsync()
    {
        const string newTemplateName = "new-template";
        const string projectPath = ".";

        var projectCommand = Substitute.For<IProjectCommand>();

        projectCommand
            .SwitchTemplateAsync(newTemplateName, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ProjectNotLoadedException("Failed to load project."));

        var sut = Ctor(projectCommand);
        var actual = await sut.SwitchTemplateAsync(newTemplateName, projectPath);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Error));
    }

    [Test]
    public async Task SwitchTemplateAsync_ProjectPathIsNull_UsesCurrentDirectoryAsync()
    {
        const string newTemplateName = "new-template";

        var projectCommand = Substitute.For<IProjectCommand>();

        projectCommand
            .SwitchTemplateAsync(newTemplateName, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var sut = Ctor(projectCommand);
        await sut.SwitchTemplateAsync(newTemplateName, projectPath: null);

        await projectCommand.Received(1).SwitchTemplateAsync(
            newTemplateName,
            Path.GetFullPath("."),
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public async Task SwitchTemplateAsync_ProjectPathIsEmpty_UsesCurrentDirectoryAsync()
    {
        const string newTemplateName = "new-template";

        var projectCommand = Substitute.For<IProjectCommand>();

        projectCommand
            .SwitchTemplateAsync(newTemplateName, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var sut = Ctor(projectCommand);
        await sut.SwitchTemplateAsync(newTemplateName, projectPath: string.Empty);

        await projectCommand.Received(1).SwitchTemplateAsync(
            newTemplateName,
            Path.GetFullPath("."),
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public async Task SwitchTemplateAsync_ResolvesFullPathForProjectPathAsync()
    {
        const string newTemplateName = "new-template";
        const string projectPath = "./sub-dir";

        var projectCommand = Substitute.For<IProjectCommand>();

        projectCommand
            .SwitchTemplateAsync(newTemplateName, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var sut = Ctor(projectCommand);
        await sut.SwitchTemplateAsync(newTemplateName, projectPath);

        await projectCommand.Received(1).SwitchTemplateAsync(
            newTemplateName,
            Path.GetFullPath(projectPath),
            Arg.Any<CancellationToken>()
        );
    }
}
