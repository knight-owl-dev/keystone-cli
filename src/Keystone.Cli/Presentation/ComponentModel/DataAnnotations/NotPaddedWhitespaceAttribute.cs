using System.ComponentModel.DataAnnotations;


namespace Keystone.Cli.Presentation.ComponentModel.DataAnnotations;

/// <summary>
/// Enforces that the value is not padded with whitespace.
/// </summary>
public sealed class NotPaddedWhitespaceAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value switch
        {
            null => ValidationResult.Success,
            string text when NotPadded(text) => ValidationResult.Success,
            string when validationContext is not null => new ValidationResult(FormatErrorMessage(validationContext.DisplayName)),
            string => new ValidationResult(errorMessage: null),
            _ => throw new NotSupportedException($"Values of type {value.GetType().FullName} are not supported."),
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
        => this.ErrorMessage ?? $"'{name}' must not be padded with whitespace.";

    private static bool NotPadded(string text)
        => text == text.Trim();
}
