using System.ComponentModel.DataAnnotations;
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
    public void GetValidationResult_WhenValueIsNull_ReturnsSuccess()
    {
        var validationContext = new ValidationContext(new object())
        {
            DisplayName = "TestPath",
        };

        var sut = new PathAttribute();
        var actual = sut.GetValidationResult(value: null, validationContext);

        Assert.That(actual, Is.EqualTo(ValidationResult.Success));
    }

    [TestCaseSource(nameof(ValidPathTestCases))]
    public void GetValidationResult_WhenValueIsValid_ReturnsSuccess(string path)
    {
        var validationContext = new ValidationContext(path)
        {
            DisplayName = nameof(path),
        };

        var sut = new PathAttribute();
        var actual = sut.GetValidationResult(path, validationContext);

        Assert.That(actual, Is.EqualTo(ValidationResult.Success));
    }

    [TestCaseSource(nameof(InvalidPathTestCases))]
    public void GetValidationResult_WhenValueIsInvalid_ReturnsError(string path)
    {
        const string displayName = "TestPath";

        var validationContext = new ValidationContext(path)
        {
            DisplayName = displayName,
        };

        var sut = new PathAttribute();
        var expectedErrorMessage = sut.FormatErrorMessage(displayName);

        var actual = sut.GetValidationResult(path, validationContext)!;

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

        var sut = new PathAttribute();

        Assert.That(
            () => sut.GetValidationResult(value, validationContext),
            Throws.TypeOf<NotSupportedException>()
                .With.Message.EqualTo($"Values of type {typeof(int).FullName} are not supported.")
        );
    }

    [Test]
    public void IsValid_SupportsLegacyValidation_ForInvalidPath()
    {
        var sut = new PathAttribute();
        var actual = sut.IsValid(string.Empty);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsValid_SupportsLegacyValidation_ForValidPath()
    {
        var sut = new PathAttribute();
        var actual = sut.IsValid("valid/path/to/directory");

        Assert.That(actual, Is.True);
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

        Assert.That(actual, Is.EqualTo($"'{name}' must be a valid path."));
    }
}
