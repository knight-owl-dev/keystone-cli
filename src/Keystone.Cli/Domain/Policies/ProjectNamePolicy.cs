namespace Keystone.Cli.Domain.Policies;

/// <summary>
/// Defines project name policies.
/// </summary>
public static class ProjectNamePolicy
{
    /// <summary>
    /// Gets the project directory name based on the provided name.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <returns>
    /// The kebab-case version of the provided name, with all delimiters replaced by hyphens.
    /// </returns>
    public static string GetProjectDirectoryName(string name)
        => KebabCaseNamingPolicy.ToKebabCase(name);
}
