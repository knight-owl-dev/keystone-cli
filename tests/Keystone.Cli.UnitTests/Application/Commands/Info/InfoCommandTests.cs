using Keystone.Cli.Application;
using Keystone.Cli.Application.Commands.Info;
using Keystone.Cli.Domain;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Commands.Info;

[TestFixture, Parallelizable(ParallelScope.All)]
public class InfoCommandTests
{
    private static InfoCommand Ctor(ITemplateService? templateService = null)
        => new(templateService ?? Substitute.For<ITemplateService>());

    [Test]
    public void GetInfo_ReturnsModel()
    {
        var defaultTemplateTarget = new TemplateTargetModel("default", new Uri("https://example.com/default"));

        TemplateTargetModel[] templateTargets =
        [
            new("template-a", new Uri("https://example.com/template-a")),
            new("template-b", new Uri("https://example.com/template-b")),
            defaultTemplateTarget,
        ];

        var templateService = Substitute.For<ITemplateService>();
        templateService.GetTemplateTarget(name: null).Returns(defaultTemplateTarget);
        templateService.GetTemplateTargets().Returns(templateTargets);

        var sut = Ctor(templateService);
        var actual = sut.GetInfo();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Version, Does.StartWith("0.2.0"));
            Assert.That(actual.Description, Is.EqualTo("A command-line interface for Keystone."));
            Assert.That(actual.Copyright, Is.EqualTo("© 2025 Knight Owl LLC. All rights reserved."));
            Assert.That(actual.DefaultTemplateTarget, Is.EqualTo(defaultTemplateTarget));
            Assert.That(actual.TemplateTargets, Is.EqualTo(templateTargets));
        }
    }
}
