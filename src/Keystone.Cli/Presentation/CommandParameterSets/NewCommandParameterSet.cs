using System.ComponentModel.DataAnnotations;
using Cocona;
using Keystone.Cli.Presentation.ComponentModel;


namespace Keystone.Cli.Presentation.CommandParameterSets;

/// <summary>
/// The parameter set for the "new" command.
/// </summary>
public record NewCommandParameterSet(
    [Argument(Description = "The name of the new project, also used as its root directory unless the project path is provided"),
     Required(AllowEmptyStrings = false, ErrorMessage = "The project name is required."),
     NotPaddedWhitespace(ErrorMessage = "The project name is padded with whitespace.")]
    string Name,
    [Option(Description = "The template name"),
     NotPaddedWhitespace(ErrorMessage = "The template name is padded with whitespace.")]
    string? TemplateName = null,
    [Option(Description = "The path where to create the new project"),
     Path(ErrorMessage = "The project path is invalid."),
     NotPaddedWhitespace(ErrorMessage = "The project path is padded with whitespace.")]
    string? ProjectPath = null
)
    : ICommandParameterSet
{
    /// <summary>
    /// Validates the command parameters.
    /// </summary>
    /// <exception cref="ValidationException">
    /// Thrown when the command parameters are invalid.
    /// </exception>
    public void Validate()
        => Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
}
