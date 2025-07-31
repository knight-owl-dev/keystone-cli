namespace Keystone.Cli.Domain;

/// <summary>
/// Standard exit codes used across all CLI commands.
/// </summary>
/// <remarks>
/// These codes are intended for use by individual command handlers but are ultimately
/// communicated back through the program's main entry point. Handlers must follow the
/// program's exit code guidelines to ensure consistent CLI behavior.
/// </remarks>
public static class CliCommandResults
{
    /// <summary>
    /// Indicates the command was successful.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// Indicates the command encountered an error.
    /// </summary>
    public const int Error = 1;

    /// <summary>
    /// Indicates the command was called with invalid arguments or parameters.
    /// </summary>
    public const int ErrorInvalidArguments = 2;
}
