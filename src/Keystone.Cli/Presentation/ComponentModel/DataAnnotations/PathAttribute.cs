using System.ComponentModel.DataAnnotations;


namespace Keystone.Cli.Presentation.ComponentModel.DataAnnotations;

/// <summary>
/// Ensures the path does not contain invalid characters.
/// </summary>
public class PathAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value switch
        {
            null => ValidationResult.Success,
            string path when IsValidPath(path) => ValidationResult.Success,
            string when validationContext is not null => new ValidationResult(FormatErrorMessage(validationContext.DisplayName)),
            string => new ValidationResult(errorMessage: null),
            _ => throw new NotSupportedException($"Values of type {value.GetType().FullName} are not supported."),
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
        => this.ErrorMessage ?? $"'{name}' must be a valid path.";

    private static bool IsValidPath(string path)
        => ! string.IsNullOrWhiteSpace(path) && path.IndexOfAny(Path.GetInvalidPathChars()) == -1;
}
