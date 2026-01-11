using Keystone.Cli.Application.Utility;


namespace Keystone.Cli.UnitTests.Application.Utility;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ContentHashServiceTests
{
    private readonly ContentHashService _sut = new();

    [Test]
    public void ComputeFromString_IsEmpty_ReturnsHash()
    {
        const string emptyStringHash = "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855";

        var actual = _sut.ComputeFromString(content: string.Empty);

        Assert.That(actual, Is.EqualTo(emptyStringHash));
    }

    [Test]
    public void ComputeFromString_IsNotEmpty_ReturnsHash()
    {
        const string content = "Hello, World!";
        const string contentHash = "DFFD6021BB2BD5B0AF676290809EC3A53191DD81C7F70A4B28688A362182986F";

        var actual = _sut.ComputeFromString(content);

        Assert.That(actual, Is.EqualTo(contentHash));
    }

    [Test]
    public void ComputeFromLines_SameInput_ProducesConsistentHash()
    {
        string[] lines = ["foo", "bar", "baz"];

        var hash1 = _sut.ComputeFromLines(lines);
        var hash2 = _sut.ComputeFromLines(lines);

        Assert.That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void ComputeFromLines_DifferentInputs_ProduceDifferentHashes()
    {
        string[] lines1 = ["foo", "bar", "baz"];
        string[] lines2 = ["foo", "baz", "bar"]; // different order

        var hash1 = _sut.ComputeFromLines(lines1);
        var hash2 = _sut.ComputeFromLines(lines2);

        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void ComputeFromKeyValues_SameInput_ProducesConsistentHash()
    {
        var dict = new Dictionary<string, string?>
        {
            ["a"] = "1",
            ["b"] = "2",
            ["c"] = null,
        };

        var hash1 = _sut.ComputeFromKeyValues(dict);
        var hash2 = _sut.ComputeFromKeyValues(dict);

        Assert.That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void ComputeFromKeyValues_DifferentInputs_ProduceDifferentHashes()
    {
        var dict1 = new Dictionary<string, string?>
        {
            ["a"] = "1",
            ["b"] = "2",
        };

        var dict2 = new Dictionary<string, string?>
        {
            ["a"] = "1",
            ["b"] = "3",
        };

        var hash1 = _sut.ComputeFromKeyValues(dict1);
        var hash2 = _sut.ComputeFromKeyValues(dict2);

        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void ComputeFromKeyValues_DifferentOrder_ProducesSameHash()
    {
        var dict1 = new Dictionary<string, string?>
        {
            ["a"] = "1",
            ["b"] = "2",
        };

        var dict2 = new Dictionary<string, string?>
        {
            ["b"] = "2",
            ["a"] = "1",
        };

        var hash1 = _sut.ComputeFromKeyValues(dict1);
        var hash2 = _sut.ComputeFromKeyValues(dict2);

        Assert.That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void ComputeFromKeyValues_NullAndEmptyValue_ProduceSameHash()
    {
        var dictWithNull = new Dictionary<string, string?>
        {
            ["a"] = null,
        };

        var dictWithEmpty = new Dictionary<string, string?>
        {
            ["a"] = "",
        };

        var hashNull = _sut.ComputeFromKeyValues(dictWithNull);
        var hashEmpty = _sut.ComputeFromKeyValues(dictWithEmpty);

        Assert.That(hashNull, Is.EqualTo(hashEmpty));
    }
}
