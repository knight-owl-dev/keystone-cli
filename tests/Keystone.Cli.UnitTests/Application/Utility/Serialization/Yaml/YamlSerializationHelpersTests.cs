using Keystone.Cli.Application.Utility.Serialization.Yaml;
using Keystone.Cli.Application.Utility.Text;


namespace Keystone.Cli.UnitTests.Application.Utility.Serialization.Yaml;

[TestFixture, Parallelizable(ParallelScope.All)]
public class YamlSerializationHelpersTests
{
    [Test]
    public void IsEqualValue_WhenScalarEntryAndMatchingScalarValue_ReturnsTrue()
    {
        var entry = new YamlParsingUtility.ScalarEntry("property", "value", ["property: value"]);
        var value = new YamlScalar("value");

        var actual = YamlSerializationHelpers.IsEqualValue(entry, value);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void IsEqualValue_WhenScalarEntryAndNonMatchingScalarValue_ReturnsFalse()
    {
        var entry = new YamlParsingUtility.ScalarEntry("property", "value1", ["property: value1"]);
        var value = new YamlScalar("value2");

        var actual = YamlSerializationHelpers.IsEqualValue(entry, value);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsEqualValue_WhenArrayEntryAndMatchingArrayValue_ReturnsTrue()
    {
        var entry = new YamlParsingUtility.ArrayEntry("property", ["a", "b"], ["- a", "- b"]);
        var value = new YamlArray(["a", "b"]);

        var actual = YamlSerializationHelpers.IsEqualValue(entry, value);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void IsEqualValue_WhenArrayEntryAndNonMatchingArrayValue_ReturnsFalse()
    {
        var entry = new YamlParsingUtility.ArrayEntry("property", ["a", "b"], ["- a", "- b"]);
        var value = new YamlArray(["a", "c"]);

        var actual = YamlSerializationHelpers.IsEqualValue(entry, value);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsEqualValue_WhenMismatchedTypes_ReturnsFalse()
    {
        var entry = new YamlParsingUtility.ScalarEntry("property", "value", ["property: value"]);
        var value = new YamlArray(["value"]);

        var actual = YamlSerializationHelpers.IsEqualValue(entry, value);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void ToEntry_WithScalarValue_ReturnsScalarEntry()
    {
        var value = new YamlScalar("val");
        var expected = new YamlParsingUtility.ScalarEntry("prop", "val", ["prop: val"]);

        var entry = YamlSerializationHelpers.ToEntry("prop", value);

        Assert.That(entry, Is.EqualTo(expected));
    }

    [Test]
    public void ToEntry_WithArrayValue_ReturnsArrayEntry()
    {
        var value = new YamlArray(["x", "y"]);
        var expected = new YamlParsingUtility.ArrayEntry("arr", ["x", "y"], ["arr:", "- x", "- y"]);

        var actual = YamlSerializationHelpers.ToEntry("arr", value);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ToEntry_WithUnsupportedType_ThrowsNotSupportedException()
    {
        var value = new DummyYamlValue();

        Assert.That(
            () => YamlSerializationHelpers.ToEntry("key", value),
            Throws.TypeOf<NotSupportedException>()
        );
    }

    [Test]
    public void AsEntry_WithScalarValue_ReturnsScalarEntry()
    {
        var kvp = new KeyValuePair<string, YamlValue>("foo", new YamlScalar("bar"));
        var expected = new YamlParsingUtility.ScalarEntry("foo", "bar", ["foo: bar"]);

        var actual = YamlSerializationHelpers.AsEntry(kvp);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void AsEntry_WithArrayValue_ReturnsArrayEntry()
    {
        var kvp = new KeyValuePair<string, YamlValue>("foo", new YamlArray(["a", "b"]));
        var expected = new YamlParsingUtility.ArrayEntry("foo", ["a", "b"], ["foo:", "- a", "- b"]);

        var actual = YamlSerializationHelpers.AsEntry(kvp);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void AsEntry_WithUnsupportedType_ThrowsNotSupportedException()
    {
        var kvp = new KeyValuePair<string, YamlValue>("foo", new DummyYamlValue());

        Assert.That(
            () => YamlSerializationHelpers.AsEntry(kvp),
            Throws.TypeOf<NotSupportedException>()
        );
    }

    [Test]
    public void GetValue_WithScalarEntry_ReturnsYamlScalar()
    {
        var entry = new YamlParsingUtility.ScalarEntry("foo", "bar", ["foo: bar"]);
        var expected = new YamlScalar("bar");

        var actual = YamlSerializationHelpers.GetValue(entry);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetValue_WithArrayEntry_ReturnsYamlArray()
    {
        var entry = new YamlParsingUtility.ArrayEntry("foo", ["a", "b"], ["- a", "- b"]);
        var expected = new YamlArray(["a", "b"]);

        var actual = YamlSerializationHelpers.GetValue(entry);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetValue_WithUnsupportedEntry_ThrowsNotSupportedException()
    {
        var entry = new DummyEntry();

        Assert.That(
            () => YamlSerializationHelpers.GetValue(entry),
            Throws.TypeOf<NotSupportedException>()
        );
    }

    private record DummyYamlValue : YamlValue;

    private record DummyEntry() : YamlParsingUtility.Entry(null, [""], YamlParsingUtility.EntryKind.Unknown);
}
