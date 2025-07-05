using Keystone.Cli.Application;
using Keystone.Cli.Application.Data;
using Keystone.Cli.Domain;
using Microsoft.Extensions.Configuration;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application;

[TestFixture, Parallelizable(ParallelScope.All)]
public class TemplateServiceTests
{
    private static TemplateService Ctor(
        IConfiguration? configuration = null,
        ITemplateTargetsRepository? templateTargetsRepository = null
    )
        => new(
            configuration ?? FakeConfiguration("default-template"),
            templateTargetsRepository ?? Substitute.For<ITemplateTargetsRepository>()
        );

    [Test]
    public void Ctor_Configuration_DefaultTemplateKey_NotSet_ThrowsInvalidOperationException()
    {
        var configuration = new ConfigurationBuilder().Build();

        Assert.That(
            () => Ctor(configuration),
            Throws.InvalidOperationException
                .With.Message.EqualTo("The 'DefaultTemplate' key is not set in the configuration.")
        );
    }

    [Test]
    public void GetTemplateTarget_Name_IsNull_ReturnsDefaultTemplateTarget()
    {
        const string defaultTemplateName = "default-template";

        var template = new TemplateTargetModel(
            defaultTemplateName,
            new Uri("https://example.com")
        );

        var templateTargetsRepository = Substitute.For<ITemplateTargetsRepository>();
        templateTargetsRepository.GetTemplateTarget(defaultTemplateName).Returns(template);

        var sut = Ctor(FakeConfiguration(defaultTemplateName), templateTargetsRepository);
        var actual = sut.GetTemplateTarget(name: null);

        Assert.That(actual, Is.EqualTo(template));
    }

    [Test]
    public void GetTemplateTarget_Name_IsWhiteSpace_ReturnsDefaultTemplateTarget()
    {
        const string defaultTemplateName = "default-template";

        var template = new TemplateTargetModel(
            defaultTemplateName,
            new Uri("https://example.com")
        );

        var templateTargetsRepository = Substitute.For<ITemplateTargetsRepository>();
        templateTargetsRepository.GetTemplateTarget(defaultTemplateName).Returns(template);

        var sut = Ctor(FakeConfiguration(defaultTemplateName), templateTargetsRepository);
        var actual = sut.GetTemplateTarget(name: "   ");

        Assert.That(actual, Is.EqualTo(template));
    }

    [Test]
    public void GetTemplateTarget_DefaultTemplateName_NotFound_ThrowsKeyNotFoundException()
    {
        const string defaultTemplateName = "default-template";
        const string templateNameA = "template-a";
        const string templateNameB = "template-b";

        var templateA = new TemplateTargetModel(
            templateNameA,
            new Uri("https://example.com/a")
        );

        var templateB = new TemplateTargetModel(
            templateNameB,
            new Uri("https://example.com/b")
        );

        var templateTargetsRepository = Substitute.For<ITemplateTargetsRepository>();

        templateTargetsRepository.GetTemplateTarget(defaultTemplateName).Returns((TemplateTargetModel?) null);
        templateTargetsRepository.GetTemplateTargets().Returns([templateA, templateB]);

        var sut = Ctor(FakeConfiguration(defaultTemplateName), templateTargetsRepository);

        Assert.That(
            () => sut.GetTemplateTarget(name: null),
            Throws.TypeOf<KeyNotFoundException>()
                .With.Message.EqualTo($"Template target '{defaultTemplateName}' not found. Supported targets: {templateNameA}, {templateNameB}.")
        );
    }

    [Test]
    public void GetTemplateTarget_Name_IsSet_ReturnsTemplateTarget()
    {
        const string templateName = "template-a";

        var template = new TemplateTargetModel(
            templateName,
            new Uri("https://example.com/a")
        );

        var templateTargetsRepository = Substitute.For<ITemplateTargetsRepository>();
        templateTargetsRepository.GetTemplateTarget(templateName).Returns(template);

        var sut = Ctor(templateTargetsRepository: templateTargetsRepository);
        var actual = sut.GetTemplateTarget(templateName);

        Assert.That(actual, Is.EqualTo(template));
    }

    [Test]
    public void GetTemplateTarget_Name_NotFound_ThrowsKeyNotFoundException()
    {
        const string templateName = "template";
        const string templateNameA = "template-a";
        const string templateNameB = "template-b";

        var templateA = new TemplateTargetModel(
            templateNameA,
            new Uri("https://example.com/a")
        );

        var templateB = new TemplateTargetModel(
            templateNameB,
            new Uri("https://example.com/b")
        );

        var templateTargetsRepository = Substitute.For<ITemplateTargetsRepository>();
        templateTargetsRepository.GetTemplateTargets().Returns([templateA, templateB]);

        var sut = Ctor(templateTargetsRepository: templateTargetsRepository);

        Assert.That(
            () => sut.GetTemplateTarget(templateName),
            Throws.TypeOf<KeyNotFoundException>()
                .With.Message.EqualTo($"Template target '{templateName}' not found. Supported targets: {templateNameA}, {templateNameB}.")
        );
    }

    [Test]
    public void GetTemplateTargets_ReturnsAllTemplateTargets()
    {
        var templateA = new TemplateTargetModel(
            "template-a",
            new Uri("https://example.com/a")
        );

        var templateB = new TemplateTargetModel(
            "template-b",
            new Uri("https://example.com/b")
        );

        var templateTargetsRepository = Substitute.For<ITemplateTargetsRepository>();
        templateTargetsRepository.GetTemplateTargets().Returns([templateA, templateB]);

        var sut = Ctor(templateTargetsRepository: templateTargetsRepository);
        var actual = sut.GetTemplateTargets();

        Assert.That(actual, Is.EqualTo([templateA, templateB]));
    }

    private static IConfiguration FakeConfiguration(string defaultTemplateName)
        => new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string?>("DefaultTemplate", defaultTemplateName)])
            .Build();
}
