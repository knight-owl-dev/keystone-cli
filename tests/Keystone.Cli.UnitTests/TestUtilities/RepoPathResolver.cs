namespace Keystone.Cli.UnitTests.TestUtilities;

/// <summary>
/// Resolves paths relative to the repository root for tests that need to access repo files.
/// </summary>
public static class RepoPathResolver
{
    private const string SolutionFileName = "keystone-cli.sln";

    /// <summary>
    /// Finds the repository root by walking up from the test's base directory.
    /// </summary>
    /// <returns>The absolute path to the repository root.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the repository root cannot be found.</exception>
    public static string GetRepoRoot()
        => FindRepoRoot(new DirectoryInfo(AppContext.BaseDirectory))
            ?? throw new InvalidOperationException("Could not find repository root. Ensure the test is running from within the repository.");

    private static string? FindRepoRoot(DirectoryInfo? directory)
        => directory switch
        {
            null => null,
            _ when File.Exists(Path.Combine(directory.FullName, SolutionFileName)) => directory.FullName,
            _ => FindRepoRoot(directory.Parent),
        };
}
