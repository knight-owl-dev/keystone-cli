using System.ComponentModel.DataAnnotations;


namespace Keystone.Cli.Presentation.ComponentModel.DataAnnotations;

/// <summary>
/// Enforces that the value is not padded with whitespace.
/// </summary>
public class NotPaddedWhitespaceAttribute : ValidationAttribute
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
        => value switch
        {
            null => true,
            string text => NotPadded(text),
            _ => throw new NotSupportedException($"Values of type {value.GetType().FullName} are not supported.")
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
        => this.ErrorMessage ?? $"{name} must not be padded with whitespace.";

    private static bool NotPadded(string text)
        => text == text.Trim();
}
