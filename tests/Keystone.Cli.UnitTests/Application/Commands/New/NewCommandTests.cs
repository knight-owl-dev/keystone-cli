using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.New;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Keystone.Cli.UnitTests.Application.Commands.New;

[TestFixture, Parallelizable(ParallelScope.All)]
public class NewCommandTests
{
    private static NewCommand Ctor(ITemplateService? templateService = null)
        => new(templateService ?? Substitute.For<ITemplateService>());

    [Test]
    public void CreateNew_ResolvesTemplateTarget()
    {
        const string name = "project-name";
        const string templateName = "template-name";

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(templateName).Throws<KeyNotFoundException>();

        var sut = Ctor(templateService);

        Assert.That(
            () => sut.CreateNew(name, templateName),
            Throws.TypeOf<KeyNotFoundException>()
        );
    }
}
