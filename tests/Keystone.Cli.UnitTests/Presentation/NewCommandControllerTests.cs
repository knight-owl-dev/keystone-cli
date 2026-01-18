using Cocona.Application;
using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Domain;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Presentation;
using Keystone.Cli.UnitTests.Application.Utility;
using Keystone.Cli.UnitTests.Presentation.Cocona;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Presentation;

[TestFixture, Parallelizable(ParallelScope.All)]
public class NewCommandControllerTests
{
    private static NewCommandController Ctor(
        ICoconaAppContextAccessor? contextAccessor = null,
        INewCommand? newCommand = null
    )
        => new(
            contextAccessor ?? Substitute.For<ICoconaAppContextAccessor>(),
            NullConsole.Instance,
            newCommand ?? Substitute.For<INewCommand>()
        );

    [Test]
    public async Task NewAsync_OnSuccess_ReturnsCliSuccessAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var sut = Ctor();
        var actual = await sut.NewAsync(name, templateName);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Success));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task NewAsync_WhenPathIsNotProvided_UsesDefaultPathAsync(string? path)
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var newCommand = Substitute.For<INewCommand>();

        var sut = Ctor(newCommand: newCommand);
        await sut.NewAsync(name, templateName, path);

        await newCommand.Received(1).CreateNewAsync(
            name,
            templateName,
            Path.Combine(Path.GetFullPath("."), ProjectNamePolicy.GetProjectDirectoryName(name)),
            includeGitFiles: false,
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public async Task NewAsync_WhenRelativePathIsProvided_UsesFullPathAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var newCommand = Substitute.For<INewCommand>();

        var sut = Ctor(newCommand: newCommand);
        await sut.NewAsync(name, templateName, projectPath: ".");

        await newCommand.Received(1).CreateNewAsync(
            name,
            templateName,
            Path.GetFullPath("."),
            includeGitFiles: false,
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public async Task NewAsync_UsesIncludeGitFilesOptionAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var newCommand = Substitute.For<INewCommand>();

        var sut = Ctor(newCommand: newCommand);
        await sut.NewAsync(name, templateName, includeGitFiles: true);

        await newCommand.Received(1).CreateNewAsync(
            name,
            templateName,
            Arg.Any<string>(),
            includeGitFiles: true,
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public async Task NewAsync_OnKeyNotFoundException_ReturnsCliErrorAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var newCommand = Substitute.For<INewCommand>();

        newCommand
            .When(stub => stub.CreateNewAsync(name, templateName, Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new KeyNotFoundException());

        var sut = Ctor(newCommand: newCommand);
        var actual = await sut.NewAsync(name, templateName);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Error));
    }

    [Test]
    public async Task NewAsync_OnInvalidOperationException_ReturnsCliErrorAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var newCommand = Substitute.For<INewCommand>();

        newCommand
            .When(stub => stub.CreateNewAsync(name, templateName, Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new InvalidOperationException());

        var sut = Ctor(newCommand: newCommand);
        var actual = await sut.NewAsync(name, templateName);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Error));
    }

    [Test]
    public async Task NewAsync_UsesCancellationTokenFromContextAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        using var cts = new CancellationTokenSource();
        var expectedToken = cts.Token;

        var context = CoconaAppContextFactory.Create(expectedToken);

        var contextAccessor = Substitute.For<ICoconaAppContextAccessor>();
        contextAccessor.Current.Returns(context);

        var newCommand = Substitute.For<INewCommand>();

        var sut = Ctor(contextAccessor, newCommand);
        await sut.NewAsync(name, templateName);

        await newCommand.Received(1).CreateNewAsync(
            name,
            templateName,
            Arg.Any<string>(),
            Arg.Any<bool>(),
            expectedToken
        );
    }
}
