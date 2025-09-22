using Keystone.Cli.Application.Utility.Text;


namespace Keystone.Cli.UnitTests.Application.Utility.Text;

[TestFixture, Parallelizable(ParallelScope.All)]
public class YamlParsingUtilityTests
{
    [Test]
    public void IsTerminatorEntry_WithTerminatorEntry_ReturnsTrue()
    {
        var entry = new YamlParsingUtility.UnknownEntry(["..."]);
        var actual = YamlParsingUtility.IsTerminatorEntry(entry);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void IsTerminatorEntry_NotTerminatorEntry_ReturnsFalse()
    {
        var entry = new YamlParsingUtility.UnknownEntry(["... # Comment"]);
        var actual = YamlParsingUtility.IsTerminatorEntry(entry);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsTerminatorEntry_WhenScalarType_ReturnsFalse()
    {
        var entry = new YamlParsingUtility.ScalarEntry("property", "...", ["property: ..."]);
        var actual = YamlParsingUtility.IsTerminatorEntry(entry);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsTerminatorLine_WithTerminatorLine_ReturnsTrue()
    {
        var actual = YamlParsingUtility.IsTerminatorLine(line: "...");

        Assert.That(actual, Is.True);
    }

    [TestCase(" ...")]
    [TestCase("... ")]
    [TestCase("... # Comment")]
    [TestCase("....")]
    [TestCase("..")]
    public void IsTerminatorLine_NotTerminatorLine_ReturnsFalse(string line)
    {
        var actual = YamlParsingUtility.IsTerminatorLine(line);

        Assert.That(actual, Is.False);
    }

    // TODO: Add tests for Parse, ToScalarEntry and ToArrayEntry methods.
}
