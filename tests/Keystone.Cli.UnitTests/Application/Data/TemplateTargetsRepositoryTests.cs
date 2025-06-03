using Keystone.Cli.Application.Data;
using Keystone.Cli.Domain;
using Microsoft.Extensions.Configuration;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Data;

[TestFixture, Parallelizable(ParallelScope.All)]
public class TemplateTargetsRepositoryTests
{
    private static TemplateTargetsRepository Ctor(IConfiguration? configuration = null)
        => new(configuration ?? Substitute.For<IConfiguration>());

    [Test]
    public void GetTemplateTargets_UndefinedConfiguration_ReturnsEmpty()
    {
        var sut = Ctor();
        var actual = sut.GetTemplateTargets();

        Assert.That(actual, Is.Empty);
    }

    [Test]
    public void GetTemplateTargets_ReturnsAllTemplateTargets()
    {
        var settings = new Dictionary<string, string>
        {
            ["Templates:template-a"] = "https://github.com/knight-owl-dev/template-a",
            ["Templates:template-b"] = "https://github.com/knight-owl-dev/template-b",
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        TemplateTargetModel[] expected =
        [
            new("template-a", new Uri("https://github.com/knight-owl-dev/template-a")),
            new("template-b", new Uri("https://github.com/knight-owl-dev/template-b")),
        ];

        var sut = Ctor(configuration);
        var actual = sut.GetTemplateTargets().ToList();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetTemplateTarget_NotFound_ReturnsNull()
    {
        const string name = "fake-template";

        var sut = Ctor();
        var actual = sut.GetTemplateTarget(name);

        Assert.That(actual, Is.Null);
    }

    [Test]
    public void GetTemplateTarget_HasFound_ReturnsTemplateTarget()
    {
        var settings = new Dictionary<string, string>
        {
            ["Templates:template-a"] = "https://github.com/knight-owl-dev/template-a",
            ["Templates:template-b"] = "https://github.com/knight-owl-dev/template-b",
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var expected = new TemplateTargetModel("template-b", new Uri("https://github.com/knight-owl-dev/template-b"));

        var sut = Ctor(configuration);
        var actual = sut.GetTemplateTarget(expected.Name);

        Assert.That(actual, Is.EqualTo(expected));
    }
}
