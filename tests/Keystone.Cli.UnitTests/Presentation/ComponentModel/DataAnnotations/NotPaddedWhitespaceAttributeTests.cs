using Keystone.Cli.Presentation.ComponentModel.DataAnnotations;


namespace Keystone.Cli.UnitTests.Presentation.ComponentModel.DataAnnotations;

[TestFixture, Parallelizable(ParallelScope.All)]
public class NotPaddedWhitespaceAttributeTests
{
    private static readonly TestCaseData<string>[] ValidTestCases =
    [
        new("text"),
        new("a"),
        new("abc def"),
        new(""),
        new("123"),
        new("a b c"),
        new("a\nb c"),
        new("a\tb c"),
        new("a\r\nb c"),
        new("a\r b c"),
        new("a\nb\tc"),
    ];

    private static readonly TestCaseData<string>[] InvalidTestCases =
    [
        new(" text"),
        new("text "),
        new(" text "),
        new("\ttext"),
        new("text\t"),
        new("\ntext"),
        new("text\n"),
        new(" text\t"),
        new("\rtext "),
        new("text\r"),
        new(" text\r\n"),
        new("\r\ntext "),
        new(" text\r\n"),
        new(" text \t"),
        new("\ttext \n"),
        new(" text \r\n"),
    ];

    [Test]
    public void IsValid_WhenValueIsNull_ReturnsTrue()
    {
        var sut = new NotPaddedWhitespaceAttribute();
        var actual = sut.IsValid(value: null);

        Assert.That(actual, Is.True);
    }

    [TestCaseSource(nameof(ValidTestCases))]
    public void IsValid_WhenValueIsNotPadded_ReturnsTrue(string value)
    {
        var sut = new NotPaddedWhitespaceAttribute();
        var actual = sut.IsValid(value);

        Assert.That(actual, Is.True);
    }

    [TestCaseSource(nameof(InvalidTestCases))]
    public void IsValid_WhenValueIsPadded_ReturnsFalse(string value)
    {
        var sut = new NotPaddedWhitespaceAttribute();
        var actual = sut.IsValid(value);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsValid_WhenValueIsNotString_ThrowsNotSupportedException()
    {
        const int value = 1234567;
        var sut = new NotPaddedWhitespaceAttribute();

        Assert.That(
            () => sut.IsValid(value),
            Throws.TypeOf<NotSupportedException>()
                .With.Message.EqualTo($"Values of type {typeof(int).FullName} are not supported.")
        );
    }

    [Test]
    public void FormatErrorMessage_WhenHasErrorMessage_ReturnsErrorMessage()
    {
        const string name = "TestField";
        const string errorMessage = "Custom error message.";

        var sut = new NotPaddedWhitespaceAttribute
        {
            ErrorMessage = errorMessage,
        };

        var actual = sut.FormatErrorMessage(name);

        Assert.That(actual, Is.EqualTo(errorMessage));
    }

    [Test]
    public void FormatErrorMessage_WhenNoErrorMessage_ReturnsDefaultErrorMessage()
    {
        const string name = "TestField";
        var sut = new NotPaddedWhitespaceAttribute();
        var actual = sut.FormatErrorMessage(name);

        Assert.That(actual, Is.EqualTo($"The {name} field must not be padded with whitespace."));
    }
}
