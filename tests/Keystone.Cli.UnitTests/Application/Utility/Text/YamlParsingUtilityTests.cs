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

    [Test]
    public void Parse_EmptyInput_ReturnsEmptyCollection()
    {
        var actual = YamlParsingUtility.Parse(lines: []).ToArray();

        Assert.That(actual, Is.Empty);
    }

    [Test]
    public void Parse_ScalarEntry_ReturnsScalarEntry()
    {
        var lines = new[] { "title: Hello World" };
        var expected = new[] { new YamlParsingUtility.ScalarEntry("title", "Hello World", lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ScalarEntry_MultiLine()
    {
        var lines = new[] { "description: |-", "  This is a description", "  that spans multiple lines." };
        var expected = new[] { new YamlParsingUtility.ScalarEntry("description", "This is a description\nthat spans multiple lines.", lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ScalarEntry_ImplicitNullValue()
    {
        var lines = new[] { "author: " };
        var expected = new[] { new YamlParsingUtility.ScalarEntry("author", Value: null, lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ScalarEntry_ExplicitNullValue()
    {
        var lines = new[] { "author: null" };
        var expected = new[] { new YamlParsingUtility.ScalarEntry("author", Value: null, lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ScalarEntry_Boolean()
    {
        var lines = new[] { "author: true" };
        var expected = new[] { new YamlParsingUtility.ScalarEntry("author", "true", lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ScalarEntry_Int()
    {
        var lines = new[] { "count: 42" };
        var expected = new[] { new YamlParsingUtility.ScalarEntry("count", "42", lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ArrayEntry_ReturnsArrayEntry()
    {
        var lines = new[] { "keywords:", "  - hello-world", "  - keystone", "  - book" };
        var expected = new[] { new YamlParsingUtility.ArrayEntry("keywords", ["hello-world", "keystone", "book"], lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ArrayEntry_HasMultiLineItem()
    {
        var lines = new[] { "notes:", "  - |-", "    This is a note", "    that spans multiple lines.", "  - Another note" };
        var expected = new[] { new YamlParsingUtility.ArrayEntry("notes", ["This is a note\nthat spans multiple lines.", "Another note"], lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ArrayEntry_InlineArray()
    {
        var lines = new[] { "tags: [tag1, tag2, tag3]" };
        var expected = new[] { new YamlParsingUtility.ArrayEntry("tags", ["tag1", "tag2", "tag3"], lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ArrayEntry_EmptyArray()
    {
        var lines = new[] { "items: []" };
        var expected = new[] { new YamlParsingUtility.ArrayEntry("items", [], lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ArrayEntry_ContainsNullItem()
    {
        var lines = new[] { "values:", "  - item1", "  - null", "  - item3" };
        var expected = new[] { new YamlParsingUtility.ArrayEntry("values", ["item1", string.Empty, "item3"], lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_CommentAndBlankLines_ReturnsUnknownEntry()
    {
        var lines = new[] { "# This is a comment", "", "   " };
        var expected = new[] { new YamlParsingUtility.UnknownEntry(lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_UnknownFormat_ReturnsUnknownEntry()
    {
        var lines = new[] { "not-a-yaml-key" };
        var expected = new[] { new YamlParsingUtility.UnknownEntry(lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_NewDocument_ReturnsUnknownEntry()
    {
        var lines = new[] { "---" };
        var expected = new[] { new YamlParsingUtility.UnknownEntry(lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_Terminator_ReturnsUnknownEntry()
    {
        var lines = new[] { "..." };
        var expected = new[] { new YamlParsingUtility.UnknownEntry(lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ComplexObjectHierarchy_ReturnsUnknownEntry()
    {
        var lines = new[] { "parent:", "  child: value" };
        var expected = new[] { new YamlParsingUtility.UnknownEntry(lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_ArrayOfObjects_ReturnsUnknownEntry()
    {
        var lines = new[] { "items:", "  - name: Item1", "    value: 10", "  - name: Item2", "    value: 20" };
        var expected = new[] { new YamlParsingUtility.UnknownEntry(lines) };

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Parse_MultipleDocuments_ReturnsAllEntries()
    {
        string[] lines =
        [
            "---",
            "# Document 1",
            "title: First Document",
            "",
            "---",
            "# Document 2",
            "title: Second Document",
            "keywords:",
            "  - example",
            "  - test",
            "",
            "---",
            "# Document 3",
            "title: Third Document",
            "description: |-",
            "  This is the third document.",
            "  It has a multi-line description.",
            "...",
        ];

        YamlParsingUtility.Entry[] expected =
        [
            new YamlParsingUtility.UnknownEntry(["---"]),
            new YamlParsingUtility.UnknownEntry(["# Document 1"]),
            new YamlParsingUtility.ScalarEntry("title", "First Document", ["title: First Document"]),
            new YamlParsingUtility.UnknownEntry([""]),
            new YamlParsingUtility.UnknownEntry(["---"]),
            new YamlParsingUtility.UnknownEntry(["# Document 2"]),
            new YamlParsingUtility.ScalarEntry("title", "Second Document", ["title: Second Document"]),
            new YamlParsingUtility.ArrayEntry("keywords", ["example", "test"], ["keywords:", "  - example", "  - test"]),
            new YamlParsingUtility.UnknownEntry([""]),
            new YamlParsingUtility.UnknownEntry(["---"]),
            new YamlParsingUtility.UnknownEntry(["# Document 3"]),
            new YamlParsingUtility.ScalarEntry("title", "Third Document", ["title: Third Document"]),
            new YamlParsingUtility.ScalarEntry(
                "description",
                "This is the third document.\nIt has a multi-line description.",
                ["description: |-", "  This is the third document.", "  It has a multi-line description."]
            ),
            new YamlParsingUtility.UnknownEntry(["..."]),
        ];

        var actual = YamlParsingUtility.Parse(lines).ToArray();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToScalarEntry_CreatesScalarEntry()
    {
        var expected = new YamlParsingUtility.ScalarEntry("author", "Knight Owl LLC", ["author: Knight Owl LLC"]);
        var actual = YamlParsingUtility.ToScalarEntry("author", "Knight Owl LLC");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToScalarEntry_CreatesScalarEntry_NullValue()
    {
        var expected = new YamlParsingUtility.ScalarEntry("author", Value: null, ["author: "]);
        var actual = YamlParsingUtility.ToScalarEntry("author", value: null);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToScalarEntry_CreatesScalarEntry_MultiLine()
    {
        var expected = new YamlParsingUtility.ScalarEntry(
            "description",
            "This is a description\nthat spans multiple lines.",
            ["description: |-", "  This is a description", "  that spans multiple lines."]
        );

        var actual = YamlParsingUtility.ToScalarEntry("description", "This is a description\nthat spans multiple lines.");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToScalarEntry_CreatesScalarEntry_Boolean()
    {
        var expected = new YamlParsingUtility.ScalarEntry("isActive", "true", ["isActive: true"]);
        var actual = YamlParsingUtility.ToScalarEntry("isActive", "true");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToScalarEntry_CreatesScalarEntry_Int()
    {
        var expected = new YamlParsingUtility.ScalarEntry("count", "42", ["count: 42"]);
        var actual = YamlParsingUtility.ToScalarEntry("count", "42");

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToArrayEntry_CreatesArrayEntry()
    {
        var items = new[] { "hello-world", "keystone", "book" };
        var expected = new YamlParsingUtility.ArrayEntry("keywords", items, ["keywords:", "  - hello-world", "  - keystone", "  - book"]);
        var actual = YamlParsingUtility.ToArrayEntry("keywords", items);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToArrayEntry_EmptyArray_CreatesEmptyArrayEntry()
    {
        var expected = new YamlParsingUtility.ArrayEntry("items", [], ["items: []"]);
        var actual = YamlParsingUtility.ToArrayEntry("items", []);

        Assert.That(actual, Is.EqualTo(expected));
    }
}
