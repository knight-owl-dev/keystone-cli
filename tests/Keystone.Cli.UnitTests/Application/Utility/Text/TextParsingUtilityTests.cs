using Keystone.Cli.Application.Utility.Text;


namespace Keystone.Cli.UnitTests.Application.Utility.Text;

[TestFixture, Parallelizable(ParallelScope.All)]
public class TextParsingUtilityTests
{
    /// <summary>
    /// Test cases for <see cref="TextParsingUtility.GetKeyValueString(KeyValuePair{string, string?})"/>.
    /// </summary>
    private static readonly TestCaseData<KeyValuePair<string, string?>>[] GetKeyValueStringTestCases =
    [
        new(new KeyValuePair<string, string?>("KEY", null))
        {
            ExpectedResult = "KEY=",
            TestName = "Null",
        },
        new(new KeyValuePair<string, string?>("KEY", "VALUE"))
        {
            ExpectedResult = "KEY=VALUE",
            TestName = "Simple",
        },
        new(new KeyValuePair<string, string?>("KEY", "VALUE=OTHER"))
        {
            ExpectedResult = "KEY=VALUE=OTHER",
            TestName = "HasEqualsSign",
        },
        new(new KeyValuePair<string, string?>("KEY", "VALUE # NOT A COMMENT"))
        {
            ExpectedResult = "KEY=\"VALUE # NOT A COMMENT\"",
            TestName = "HasHashSign",
        },
        new(new KeyValuePair<string, string?>("KEY", "VALUE WITH SPACES"))
        {
            ExpectedResult = "KEY=\"VALUE WITH SPACES\"",
            TestName = "HasSpaces",
        },
        new(new KeyValuePair<string, string?>("KEY", " VALUE "))
        {
            ExpectedResult = "KEY=\" VALUE \"",
            TestName = "Padded",
        },
        new(new KeyValuePair<string, string?>("KEY", "\"VALUE\""))
        {
            ExpectedResult = "KEY=\"\"VALUE\"\"",
            TestName = "Quoted",
        },
        new(new KeyValuePair<string, string?>("KEY", "IT'S"))
        {
            ExpectedResult = "KEY=\"IT'S\"",
            TestName = "Apostrophe",
        },
        new(new KeyValuePair<string, string?>("KEY", "VALUE [\r\n\t\\]"))
        {
            ExpectedResult = "KEY=\"VALUE [\\r\\n\\t\\]\"",
            TestName = "ControlCharacters",
        },
        new(new KeyValuePair<string, string?>("KEY", @"VALUE [\r\n\t\]"))
        {
            ExpectedResult = @"KEY='VALUE [\r\n\t\]'",
            TestName = "LiteralControlCharacters",
        },
    ];

    /// <summary>
    /// Test cases for <see cref="TextParsingUtility.ParseKeyValuePair(string)"/>.
    /// </summary>
    private static readonly TestCaseData<string>[] ParseKeyValuePairTestCases =
    [
        new("KEY=")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", null),
            TestName = "EmptyValue",
        },
        new("KEY=VALUE")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE"),
            TestName = "Simple",
        },
        new(" KEY = VALUE ")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE"),
            TestName = "Padded",
        },
        new("KEY=VALUE=OTHER")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE=OTHER"),
            TestName = "HasEqualsSign",
        },
        new("KEY = \"  PADDED VALUE WITH SPACES  \"")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "  PADDED VALUE WITH SPACES  "),
            TestName = "Quoted",
        },
        new("KEY = '  PADDED VALUE WITH SPACES  '")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "  PADDED VALUE WITH SPACES  "),
            TestName = "SingleQuoted",
        },
        new("KEY=\"VALUE WITH 'APOSTROPHE'\"")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE WITH 'APOSTROPHE'"),
            TestName = "QuotedWithApostrophe",
        },
        new("KEY=\"VALUE WITH \\\"ESCAPED QUOTE\\\"\"")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE WITH \"ESCAPED QUOTE\""),
            TestName = "QuotedWithEscapedQuote",
        },
        new("KEY=\"VALUE [\\r\\n\\t\\]")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE [\r\n\t\\]"),
            TestName = "QuotedWithControlCharacters",
        },
        new(@"KEY='VALUE [\r\n\t\]'")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", @"VALUE [\r\n\t\]"),
            TestName = "SingleQuotedWithLiteralControlCharacters",
        },
        new("KEY=VALUE# NOT A COMMENT")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE# NOT A COMMENT"),
            TestName = "UnquotedValueWithHash",
        },
        new("KEY=\"VALUE # NOT A COMMENT\"")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE # NOT A COMMENT"),
            TestName = "QuotedValueWithHash",
        },
        new("KEY='VALUE # NOT A COMMENT'")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE # NOT A COMMENT"),
            TestName = "SingleQuotedValueWithHash",
        },
        new("KEY=VALUE # COMMENT")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE"),
            TestName = "UnquotedValueWithComment",
        },
        new("KEY=\"VALUE # NOT A COMMENT\" # COMMENT")
        {
            ExpectedResult = new KeyValuePair<string, string?>("KEY", "VALUE # NOT A COMMENT"),
            TestName = "QuotedValueWithComment",
        },
    ];

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

    [TestCaseSource(nameof(GetKeyValueStringTestCases))]
    public string GetKeyValueString(KeyValuePair<string, string?> keyValuePair)
        => TextParsingUtility.GetKeyValueString(keyValuePair);

    [TestCaseSource(nameof(ParseKeyValuePairTestCases))]
    public KeyValuePair<string, string?> ParseKeyValuePair(string line)
        => TextParsingUtility.ParseKeyValuePair(line);

    [Test]
    public void TryParseKeyValuePair_CommentLine_ReturnsFalse()
    {
        const string line = "# FOO=BAR";
        var success = TextParsingUtility.TryParseKeyValuePair(line, out var keyValuePair);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.False);
            Assert.That(keyValuePair, Is.Default);
        }
    }

    [Test]
    public void TryParseKeyValuePair_ValidLine_ReturnsTrueAndExpectedKvp()
    {
        const string line = "FOO=BAR";
        var success = TextParsingUtility.TryParseKeyValuePair(line, out var keyValuePair);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(keyValuePair, Is.EqualTo(new KeyValuePair<string, string?>("FOO", "BAR")));
        }
    }

    [Test]
    public void TryParseKeyValuePair_InvalidLine_ReturnsFalse()
    {
        const string line = "not-a-key-value";
        var success = TextParsingUtility.TryParseKeyValuePair(line, out var keyValuePair);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.False);
            Assert.That(keyValuePair, Is.Default);
        }
    }
}
