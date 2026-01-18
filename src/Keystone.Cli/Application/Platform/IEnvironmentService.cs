namespace Keystone.Cli.Application.Platform;

/// <summary>
/// Defines a service for accessing environment and system information.
/// </summary>
/// <remarks>
/// This interface wraps system calls to provide a testable contract for
/// environment-specific operations.
/// </remarks>
public interface IEnvironmentService
{
    /// <summary>
    /// Gets a value indicating whether the current operating system is Windows.
    /// </summary>
    bool IsWindows { get; }

    /// <summary>
    /// Gets the path of the directory containing the application executable.
    /// </summary>
    /// <seealso cref="AppContext.BaseDirectory"/>
    string AppBaseDirectory { get; }

    /// <summary>
    /// Retrieves the value of an environment variable.
    /// </summary>
    /// <param name="name">The name of the environment variable.</param>
    /// <returns>
    /// The value of the environment variable, or <c>null</c> if the variable is not found.
    /// </returns>
    /// <seealso cref="Environment.GetEnvironmentVariable(string)"/>
    string? GetEnvironmentVariable(string name);

    /// <summary>
    /// Gets the path to the specified system special folder.
    /// </summary>
    /// <param name="folder">The special folder to retrieve.</param>
    /// <returns>The path to the specified folder.</returns>
    /// <seealso cref="Environment.GetFolderPath(Environment.SpecialFolder)"/>
    string GetFolderPath(Environment.SpecialFolder folder);
}
