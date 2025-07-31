using System.ComponentModel.DataAnnotations;


namespace Keystone.Cli.Presentation.ComponentModel;

/// <summary>
/// Ensures the path does not contain invalid characters.
/// </summary>
public class PathAttribute : ValidationAttribute
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
        => value switch
        {
            null => true,
            string path => IsValidPath(path),
            _ => throw new NotSupportedException($"Values of type {value.GetType().FullName} are not supported.")
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
        => this.ErrorMessage ?? $"The {name} field must be a valid path.";

    private static bool IsValidPath(string path)
        => ! string.IsNullOrWhiteSpace(path) && ! path.Any(c => Path.GetInvalidPathChars().Contains(c));
}
