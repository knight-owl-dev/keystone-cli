namespace Keystone.Cli.Application.Platform;

/// <summary>
/// Provides access to environment and system information.
/// </summary>
public class EnvironmentService : IEnvironmentService
{
    /// <inheritdoc />
    public bool IsWindows => OperatingSystem.IsWindows();

    /// <inheritdoc />
    public string AppBaseDirectory => AppContext.BaseDirectory;

    /// <inheritdoc />
    public string? GetEnvironmentVariable(string name)
        => Environment.GetEnvironmentVariable(name);

    /// <inheritdoc />
    public string GetFolderPath(Environment.SpecialFolder folder)
        => Environment.GetFolderPath(folder);
}
