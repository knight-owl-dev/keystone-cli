using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Domain;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Presentation;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Presentation;

[TestFixture, Parallelizable(ParallelScope.All)]
public class NewCommandControllerTests
{
    private static NewCommandController Ctor(INewCommand? newCommand = null)
        => new(newCommand ?? Substitute.For<INewCommand>());

    [Test]
    public async Task NewAsync_OnSuccess_ReturnsCliSuccessAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var sut = Ctor();
        var actual = await sut.NewAsync(name, templateName, path: null);

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

        var sut = Ctor(newCommand);
        await sut.NewAsync(name, templateName, path);

        await newCommand.Received(1).CreateNewAsync(
            name,
            templateName,
            Path.Combine(Path.GetFullPath("."), ProjectNamePolicy.GetProjectDirectoryName(name)),
            CancellationToken.None
        );
    }

    [Test]
    public async Task NewAsync_WhenRelativePathIsProvided_UsesFullPathAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var newCommand = Substitute.For<INewCommand>();

        var sut = Ctor(newCommand);
        await sut.NewAsync(name, templateName, path: ".");

        await newCommand.Received(1).CreateNewAsync(
            name,
            templateName,
            Path.GetFullPath("."),
            CancellationToken.None
        );
    }

    [Test]
    public async Task NewAsync_OnKeyNotFoundException_ReturnsCliErrorAsync()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var newCommand = Substitute.For<INewCommand>();

        newCommand
            .When(stub => stub.CreateNewAsync(name, templateName, Arg.Any<string>(), CancellationToken.None))
            .Do(_ => throw new KeyNotFoundException());

        var sut = Ctor(newCommand);
        var actual = await sut.NewAsync(name, templateName, path: null);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Error));
    }
}
