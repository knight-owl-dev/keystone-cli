namespace Keystone.Cli.Domain.Project;

/// <summary>
/// Indicates that a <see cref="ProjectModel"/> could not be loaded because the project
/// is not properly initialized or the necessary files are missing.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The optional exception that caused the error.</param>
public class ProjectNotLoadedException(string message, Exception? innerException = null)
    : Exception(message, innerException);
