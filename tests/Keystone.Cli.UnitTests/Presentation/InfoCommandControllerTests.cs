using Keystone.Cli.Application.Commands.Info;
using Keystone.Cli.Domain;
using Keystone.Cli.Presentation;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Presentation;

[TestFixture, Parallelizable(ParallelScope.All)]
public class InfoCommandControllerTests
{
    private static InfoCommandController Ctor(IInfoCommand? infoCommand = null)
        => new(infoCommand ?? Substitute.For<IInfoCommand>());

    [Test]
    public void Info_ExecutesCommand()
    {
        var info = new InfoModel(
            Version: null,
            Description: null,
            Copyright: null,
            DefaultTemplateTarget: new TemplateTargetModel("default", new Uri("https://example.com/default")),
            TemplateTargets: new List<TemplateTargetModel>
            {
                new("default", new Uri("https://example.com/default")),
            }
        );

        var infoCommand = Substitute.For<IInfoCommand>();
        infoCommand.GetInfo().Returns(info);

        var sut = Ctor(infoCommand);
        sut.Info();

        infoCommand.Received(1).GetInfo();
    }
}
