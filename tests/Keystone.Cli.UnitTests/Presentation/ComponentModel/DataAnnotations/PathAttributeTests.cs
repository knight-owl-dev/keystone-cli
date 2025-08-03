using Keystone.Cli.Presentation.ComponentModel.DataAnnotations;


namespace Keystone.Cli.UnitTests.Presentation.ComponentModel.DataAnnotations;

[TestFixture, Parallelizable(ParallelScope.All)]
public class PathAttributeTests
{
    private static readonly TestCaseData<string>[] ValidPathTestCases =
    [
        new("."),
        new("~"),
        new("path/to/directory"),
        new("/path/to/directory"),
        new(@"C:\path\to\directory"),
        new("./path/to/directory"),
        new("~/path/to/directory"),
        new("./path/to/directory/"),
        new(" ./path/to/directory "),
    ];

    private static readonly TestCaseData<string>[] InvalidPathTestCases =
    [
        new(string.Empty), new(" "), ..Path.GetInvalidPathChars().Select(c => new TestCaseData<string>($"./path/to/directory/{c}")),
    ];

    [Test]
    public void IsValid_WhenValueIsNull_ReturnsTrue()
    {
        var sut = new PathAttribute();
        var actual = sut.IsValid(value: null);

        Assert.That(actual, Is.True);
    }

    [TestCaseSource(nameof(ValidPathTestCases))]
    public void IsValid_WhenValueIsValid_ReturnsTrue(string path)
    {
        var sut = new PathAttribute();
        var actual = sut.IsValid(path);

        Assert.That(actual, Is.True);
    }

    [TestCaseSource(nameof(InvalidPathTestCases))]
    public void IsValid_WhenValueIsInvalid_ReturnsFalse(string path)
    {
        var sut = new PathAttribute();
        var actual = sut.IsValid(path);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsValid_WhenValueIsNotString_ThrowsNotSupportedException()
    {
        const int value = 1234567;
        var sut = new PathAttribute();

        Assert.That(
            () => sut.IsValid(value),
            Throws.TypeOf<NotSupportedException>()
                .With.Message.EqualTo($"Values of type {typeof(int).FullName} are not supported.")
        );
    }

    [Test]
    public void FormatErrorMessage_WhenHasErrorMessage_ReturnsErrorMessage()
    {
        const string name = "TestPath";
        const string errorMessage = "Custom error message.";

        var sut = new PathAttribute
        {
            ErrorMessage = errorMessage,
        };

        var actual = sut.FormatErrorMessage(name);

        Assert.That(actual, Is.EqualTo(errorMessage));
    }

    [Test]
    public void FormatErrorMessage_WhenNoErrorMessage_ReturnsDefaultErrorMessage()
    {
        const string name = "TestPath";

        var sut = new PathAttribute();
        var actual = sut.FormatErrorMessage(name);

        Assert.That(actual, Is.EqualTo($"{name} must be a valid path."));
    }
}
