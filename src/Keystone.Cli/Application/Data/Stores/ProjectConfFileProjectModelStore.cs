using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Data.Stores;

/// <summary>
/// An implementation of <see cref="IProjectModelStore"/> that binds various <see cref="ProjectModel"/> properties
/// to a <see cref="ProjectFiles.ProjectConfFileName"/> file in the project directory.
/// </summary>
public class ProjectConfFileProjectModelStore(
    IContentHashService contentHashService,
    IFileSystemService fileSystemService,
    IEnvironmentFileSerializer environmentFileSerializer
)
    : IProjectModelStore
{
    private const string KeystoneProject = "KEYSTONE_PROJECT";
    private const string KeystoneDockerComposeProject = "KEYSTONE_DOCKER_COMPOSE_PROJECT";
    private const string KeystoneDockerImage = "KEYSTONE_DOCKER_IMAGE";

    /// <inheritdoc />
    public async Task<ProjectModel> LoadAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var projectConfFilePath = GetProjectConfFilePath(model);

        if (!fileSystemService.FileExists(projectConfFilePath))
        {
            return model;
        }

        var envValues = await environmentFileSerializer.LoadAsync(projectConfFilePath, cancellationToken);

        return model with
        {
            ProjectName = GetValueOrDefault(envValues, KeystoneProject),
            DockerComposeProject = GetValueOrDefault(envValues, KeystoneDockerComposeProject),
            DockerImage = GetValueOrDefault(envValues, KeystoneDockerImage),
        };
    }

    /// <inheritdoc />
    public Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var projectConfFilePath = GetProjectConfFilePath(model);

        var envValues = new Dictionary<string, string?>
        {
            [KeystoneProject] = model.ProjectName,
            [KeystoneDockerComposeProject] = model.DockerComposeProject,
            [KeystoneDockerImage] = model.DockerImage,
        };

        return environmentFileSerializer.SaveAsync(projectConfFilePath, envValues, cancellationToken);
    }

    /// <inheritdoc />
    public string GetContentHash(ProjectModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var envValues = new Dictionary<string, string?>
        {
            [KeystoneProject] = model.ProjectName,
            [KeystoneDockerComposeProject] = model.DockerComposeProject,
            [KeystoneDockerImage] = model.DockerImage,
        };

        return contentHashService.ComputeFromKeyValues(envValues);
    }

    /// <summary>
    /// Gets the full path to the <c>project.conf</c> file in the given project model.
    /// </summary>
    /// <param name="model">The project model.</param>
    /// <returns>The full path to the project.conf file.</returns>
    private static string GetProjectConfFilePath(ProjectModel model)
        => Path.Combine(model.ProjectPath, ProjectFiles.ProjectConfFileName);

    /// <summary>
    /// Gets the value for the specified key from the environment values dictionary,
    /// or returns null if the key doesn't exist.
    /// </summary>
    /// <param name="envValues">The environment values dictionary.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value or null if not found.</returns>
    private static string? GetValueOrDefault(IDictionary<string, string?> envValues, string key)
        => envValues.TryGetValue(key, out var value) ? value : null;
}
