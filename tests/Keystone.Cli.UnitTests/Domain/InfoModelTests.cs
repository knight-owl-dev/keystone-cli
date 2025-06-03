using Keystone.Cli.Domain;


namespace Keystone.Cli.UnitTests.Domain;

[TestFixture, Parallelizable(ParallelScope.All)]
public class InfoModelTests
{
    [Test]
    public void GetFormattedText_ReturnsFormattedText()
    {
        const string version = "1.0.0";
        const string description = "Test CLI application.";
        const string copyright = "© 2025 Test Company";

        var defaultTarget = new TemplateTargetModel("default", new Uri("https://example.com/default"));

        var targets = new List<TemplateTargetModel>
        {
            new("target1", new Uri("https://example.com/target1")),
            new("target2", new Uri("https://example.com/target2")),
            defaultTarget,
        };

        var expected = $"""
            Keystone CLI v{version}. {copyright}
            {description}

            Available Keystone template targets:
             - target1: https://example.com/target1
             - target2: https://example.com/target2
             - default: https://example.com/default

            Default template: {defaultTarget.Name}
            """;

        var sut = new InfoModel(version, description, copyright, defaultTarget, targets);

        var actual = sut.GetFormattedText();

        Assert.That(actual, Is.EqualTo(expected).IgnoreWhiteSpace);
    }

    [Test]
    public void GetFormattedText_PadsTargetTemplateNames()
    {
        var sut = new InfoModel(
            "1.0.0",
            "Test CLI application.",
            "© 2023 Test Company",
            new TemplateTargetModel("short", new Uri("https://example.com/short")),
            new List<TemplateTargetModel>
            {
                new("short", new Uri("https://example.com/short")),
                new("longer-target-name", new Uri("https://example.com/longer-target-name")),
            }
        );

        var actual = sut.GetFormattedText();

        Assert.Multiple(() =>
            {
                Assert.That(actual, Does.Contain(" -              short: https://example.com/short"));
                Assert.That(actual, Does.Contain(" - longer-target-name: https://example.com/longer-target-name"));
            }
        );
    }
}
