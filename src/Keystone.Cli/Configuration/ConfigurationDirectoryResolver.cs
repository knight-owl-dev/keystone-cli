using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Platform;
using Keystone.Cli.Domain;


namespace Keystone.Cli.Configuration;

/// <summary>
/// Resolves the configuration directory using a lookup chain.
/// </summary>
/// <remarks>
/// The lookup order (first match wins):
/// <list type="number">
///   <item><c>KEYSTONE_CLI_CONFIG_DIR</c> environment variable (all platforms)</item>
///   <item>User config directory (all platforms): <c>%APPDATA%\keystone-cli\</c> on Windows,
///         <c>~/.config/keystone-cli/</c> on Linux/macOS</item>
///   <item><c>/etc/keystone-cli/</c> (FHS system-wide, non-Windows only)</item>
///   <item><c>AppContext.BaseDirectory</c> (fallback)</item>
/// </list>
/// </remarks>
public sealed class ConfigurationDirectoryResolver(IEnvironmentService environmentService, IFileSystemService fileSystemService)
{
    /// <summary>
    /// The name of the configuration file to look for in each directory.
    /// </summary>
    private const string ConfigFileName = "appsettings.json";

    /// <summary>
    /// The environment variable that overrides the configuration directory lookup.
    /// </summary>
    private const string EnvVarName = "KEYSTONE_CLI_CONFIG_DIR";

    /// <summary>
    /// Gets the default instance using the real file system and environment services.
    /// </summary>
    /// <remarks>
    /// This instance is intended for use in <c>Program.cs</c> before DI is available.
    /// </remarks>
    public static readonly ConfigurationDirectoryResolver Default =
        new(new EnvironmentService(), new FileSystemService());

    /// <summary>
    /// Resolves the configuration directory using the lookup chain.
    /// </summary>
    /// <returns>The path to the directory containing the configuration files.</returns>
    public string ResolveConfigDirectory()
    {
        // 1. Environment variable (all platforms)
        var envPath = environmentService.GetEnvironmentVariable(EnvVarName);
        if (! string.IsNullOrEmpty(envPath) && ContainsConfigFile(envPath))
        {
            return envPath;
        }

        // 2. User config directory (all platforms)
        var userConfigPath = Path.Combine(
            environmentService.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            CliInfo.AppName
        );

        if (ContainsConfigFile(userConfigPath))
        {
            return userConfigPath;
        }

        if (environmentService.IsWindows)
        {
            return environmentService.AppBaseDirectory;
        }

        // 3. FHS: /etc/keystone-cli/ or fallback to AppBaseDirectory
        var fhsPath = Path.Combine("/etc", CliInfo.AppName);

        return ContainsConfigFile(fhsPath) ? fhsPath : environmentService.AppBaseDirectory;
    }

    /// <summary>
    /// Checks whether the specified directory contains the configuration file.
    /// </summary>
    /// <param name="directory">The directory path to check.</param>
    /// <returns>
    /// <c>true</c> if the directory contains the configuration file; otherwise, <c>false</c>.
    /// </returns>
    private bool ContainsConfigFile(string directory)
        => fileSystemService.FileExists(Path.Combine(directory, ConfigFileName));
}
