namespace Keystone.Cli.Domain.Project;

/// <summary>
/// Indicates that a <see cref="ProjectModel"/> could not be loaded because the project
/// is not properly initialized or the necessary files are missing.
/// </summary>
public class ProjectNotLoadedException : Exception
{
    /// <summary>
    /// Creates a new instance of the <see cref="ProjectNotLoadedException"/> class
    /// with the default error message.
    /// </summary>
    public ProjectNotLoadedException()
        : base("The project is not loaded.")
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ProjectNotLoadedException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public ProjectNotLoadedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ProjectNotLoadedException"/> class
    /// with a specified error message and a reference to the inner exception that
    /// is the cause of this exception.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    /// <param name="innerException">The exception that is the cause of this error.</param>
    public ProjectNotLoadedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
