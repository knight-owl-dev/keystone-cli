using Keystone.Cli.Application.Commands.New;
using Keystone.Cli.Domain;
using Keystone.Cli.Presentation;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Presentation;

[TestFixture, Parallelizable(ParallelScope.All)]
public class NewCommandControllerTests
{
    private static NewCommandController Ctor(INewCommand? newCommand = null)
        => new(newCommand ?? Substitute.For<INewCommand>());

    [Test]
    public void New_OnSuccess_ReturnsCliSuccess()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var sut = Ctor();
        var actual = sut.New(name, templateName);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Success));
    }

    [Test]
    public void New_OnKeyNotFoundException_ReturnsCliError()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var newCommand = Substitute.For<INewCommand>();

        newCommand
            .When(stub => stub.CreateNew(name, templateName))
            .Do(_ => throw new KeyNotFoundException());

        var sut = Ctor(newCommand);
        var actual = sut.New(name, templateName);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Error));
    }
}
