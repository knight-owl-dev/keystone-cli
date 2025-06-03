using Keystone.Cli.Application.Commands.Browse;
using Keystone.Cli.Domain;
using Keystone.Cli.Presentation;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Presentation;

[TestFixture, Parallelizable(ParallelScope.All)]
public class BrowseCommandControllerTests
{
    private static BrowseCommandController Ctor(IBrowseCommand? browseCommand = null)
        => new(browseCommand ?? Substitute.For<IBrowseCommand>());

    [Test]
    public void Browse_OnSuccess_ReturnsCliSuccess()
    {
        const string templateName = "template-name";

        var sut = Ctor();
        var actual = sut.Browse(templateName);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Success));
    }

    [Test]
    public void Browse_OnKeyNotFoundException_ReturnsCliError()
    {
        const string templateName = "template-name";

        var browseCommand = Substitute.For<IBrowseCommand>();

        browseCommand
            .When(stub => stub.Browse(templateName))
            .Do(_ => throw new KeyNotFoundException());

        var sut = Ctor(browseCommand);
        var actual = sut.Browse(templateName);

        Assert.That(actual, Is.EqualTo(CliCommandResults.Error));
    }
}
