using System.ComponentModel.DataAnnotations;
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
    public void GetValidationResult_WhenValueIsNull_ReturnsSuccess()
    {
        var validationContext = new ValidationContext(new object())
        {
            DisplayName = "TestField",
        };

        var sut = new NotPaddedWhitespaceAttribute();
        var actual = sut.GetValidationResult(value: null, validationContext);

        Assert.That(actual, Is.EqualTo(ValidationResult.Success));
    }

    [TestCaseSource(nameof(ValidTestCases))]
    public void GetValidationResult_WhenValueIsNotPadded_ReturnsSuccess(string value)
    {
        var validationContext = new ValidationContext(value)
        {
            DisplayName = nameof(value),
        };

        var sut = new NotPaddedWhitespaceAttribute();
        var actual = sut.GetValidationResult(value, validationContext);

        Assert.That(actual, Is.EqualTo(ValidationResult.Success));
    }

    [TestCaseSource(nameof(InvalidTestCases))]
    public void GetValidationResult_WhenValueIsPadded_ReturnsError(string value)
    {
        const string displayName = "value";

        var validationContext = new ValidationContext(value)
        {
            DisplayName = displayName,
        };

        var sut = new NotPaddedWhitespaceAttribute();
        var expectedErrorMessage = sut.FormatErrorMessage(displayName);

        var actual = sut.GetValidationResult(value, validationContext)!;

        Assert.That(actual.ErrorMessage, Is.EqualTo(expectedErrorMessage));
    }

    [Test]
    public void GetValidationResult_WhenValueIsNotString_ThrowsNotSupportedException()
    {
        const int value = 1234567;

        var validationContext = new ValidationContext(value)
        {
            DisplayName = nameof(value),
        };

        var sut = new NotPaddedWhitespaceAttribute();

        Assert.That(
            () => sut.GetValidationResult(value, validationContext),
            Throws.TypeOf<NotSupportedException>()
                .With.Message.EqualTo($"Values of type {typeof(int).FullName} are not supported.")
        );
    }

    [Test]
    public void IsValid_SupportsLegacyValidation_ForInvalidValue()
    {
        var sut = new NotPaddedWhitespaceAttribute();
        var actual = sut.IsValid(" padded text ");

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsValid_SupportsLegacyValidation_ForValidValue()
    {
        var sut = new NotPaddedWhitespaceAttribute();
        var actual = sut.IsValid("valid text");

        Assert.That(actual, Is.True);
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

        Assert.That(actual, Is.EqualTo($"'{name}' must not be padded with whitespace."));
    }
}
