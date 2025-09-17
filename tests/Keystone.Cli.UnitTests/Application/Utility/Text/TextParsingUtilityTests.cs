using Keystone.Cli.Application.Utility.Text;


namespace Keystone.Cli.UnitTests.Application.Utility.Text;

[TestFixture, Parallelizable(ParallelScope.All)]
public class TextParsingUtilityTests
{
    [Test]
    public void IsWhiteSpaceOrCommentLine_IsNull_ReturnsTrue()
    {
        const string? line = null;

        var actual = TextParsingUtility.IsWhiteSpaceOrCommentLine(line!);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void IsWhiteSpaceOrCommentLine_IsBlank_ReturnsTrue()
    {
        const string line = "   ";

        var actual = TextParsingUtility.IsWhiteSpaceOrCommentLine(line);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void IsWhiteSpaceOrCommentLine_IsComment_ReturnsTrue()
    {
        const string line = "# This is a comment";

        var actual = TextParsingUtility.IsWhiteSpaceOrCommentLine(line);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void IsWhiteSpaceOrCommentLine_NormalLine_ReturnsFalse()
    {
        const string line = "KEY=VALUE";

        var actual = TextParsingUtility.IsWhiteSpaceOrCommentLine(line);

        Assert.That(actual, Is.False);
    }

    [TestCase("KEY=VALUE", "KEY", "VALUE")]
    [TestCase(" KEY = VALUE ", "KEY", "VALUE")]
    [TestCase("KEY= VALUE ", "KEY", "VALUE")]
    [TestCase(" KEY =VALUE", "KEY", "VALUE")]
    [TestCase("KEY=VAL=UE", "KEY", "VAL=UE")]
    public void ParseKeyValuePair_ValidCases_ReturnsExpected(string line, string expectedKey, string expectedValue)
    {
        var expected = new KeyValuePair<string, string?>(expectedKey, expectedValue);
        var actual = TextParsingUtility.ParseKeyValuePair(line);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ParseKeyValuePair_KeyWithEmptyValue_ReturnsKeyAndNullValue()
    {
        var expected = new KeyValuePair<string, string?>("KEY", value: null);
        var actual = TextParsingUtility.ParseKeyValuePair("KEY=");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("", TestName = "ParseKeyValuePair_EmptyString_ReturnsDefault")]
    [TestCase("   ", TestName = "ParseKeyValuePair_WhitespaceOnly_ReturnsDefault")]
    [TestCase("NoEqualsSign", TestName = "ParseKeyValuePair_NoEqualsSign_ReturnsDefault")]
    [TestCase("=NoKey", TestName = "ParseKeyValuePair_SeparatorAtStart_ReturnsDefault")]
    public void ParseKeyValuePair_InvalidCases_ReturnsDefault(string line)
    {
        var expected = default(KeyValuePair<string, string?>);
        var actual = TextParsingUtility.ParseKeyValuePair(line);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Normalize_EmptyString_ReturnsNull()
    {
        var actual = TextParsingUtility.Normalize("");

        Assert.That(actual, Is.Null);
    }

    [Test]
    public void Normalize_WhitespaceOnly_ReturnsNull()
    {
        var actual = TextParsingUtility.Normalize("   ");

        Assert.That(actual, Is.Null);
    }

    [Test]
    public void Normalize_Null_ReturnsNull()
    {
        var actual = TextParsingUtility.Normalize(null!);

        Assert.That(actual, Is.Null);
    }

    [TestCase("value", "value")]
    [TestCase(" value ", "value")]
    [TestCase("  value", "value")]
    [TestCase("value  ", "value")]
    public void Normalize_ReturnsExpected(string input, string expected)
    {
        var actual = TextParsingUtility.Normalize(input);

        Assert.That(actual, Is.EqualTo(expected));
    }
}
