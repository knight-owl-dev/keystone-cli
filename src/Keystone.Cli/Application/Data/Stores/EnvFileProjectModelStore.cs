using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Data.Stores;

/// <summary>
/// An implementation of <see cref="IProjectModelStore"/> that binds various <see cref="ProjectModel"/> properties
/// to a <see cref="ProjectFiles.EnvFileName"/> file in the project directory.
/// </summary>
public class EnvFileProjectModelStore(
    IContentHashService contentHashService,
    IFileSystemService fileSystemService,
    IEnvironmentFileSerializer environmentFileSerializer
)
    : IProjectModelStore
{
    private const string KeystoneProject = "KEYSTONE_PROJECT";
    private const string KeystoneCoverImage = "KEYSTONE_COVER_IMAGE";
    private const string KeystoneLatexPaperSize = "KEYSTONE_LATEX_PAPERSIZE";
    private const string KeystoneLatexGeometry = "KEYSTONE_LATEX_GEOMETRY";
    private const string KeystoneLatexFontsize = "KEYSTONE_LATEX_FONTSIZE";
    private const string KeystoneLatexFontFamily = "KEYSTONE_LATEX_FONTFAMILY";
    private const string KeystoneDockerComposeProject = "KEYSTONE_DOCKER_COMPOSE_PROJECT";
    private const string KeystoneDockerImage = "KEYSTONE_DOCKER_IMAGE";

    /// <inheritdoc />
    public async Task<ProjectModel> LoadAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var envFilePath = GetEnvFilePath(model);

        if (! fileSystemService.FileExists(envFilePath))
        {
            return model;
        }

        var envValues = await environmentFileSerializer.LoadAsync(envFilePath, cancellationToken);

        return model with
        {
            ProjectName = GetValueOrDefault(envValues, KeystoneProject),
            CoverImage = GetValueOrDefault(envValues, KeystoneCoverImage),
            LatexPapersize = GetValueOrDefault(envValues, KeystoneLatexPaperSize),
            LatexGeometry = GetValueOrDefault(envValues, KeystoneLatexGeometry),
            LatexFontsize = GetValueOrDefault(envValues, KeystoneLatexFontsize),
            LatexFontfamily = GetValueOrDefault(envValues, KeystoneLatexFontFamily),
            DockerComposeProject = GetValueOrDefault(envValues, KeystoneDockerComposeProject),
            DockerImage = GetValueOrDefault(envValues, KeystoneDockerImage),
        };
    }

    /// <inheritdoc />
    public Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var envFilePath = GetEnvFilePath(model);

        var envValues = new Dictionary<string, string?>
        {
            [KeystoneProject] = model.ProjectName,
            [KeystoneCoverImage] = model.CoverImage,
            [KeystoneLatexPaperSize] = model.LatexPapersize,
            [KeystoneLatexGeometry] = model.LatexGeometry,
            [KeystoneLatexFontsize] = model.LatexFontsize,
            [KeystoneLatexFontFamily] = model.LatexFontfamily,
            [KeystoneDockerComposeProject] = model.DockerComposeProject,
            [KeystoneDockerImage] = model.DockerImage,
        };

        return environmentFileSerializer.SaveAsync(envFilePath, envValues, cancellationToken);
    }

    /// <inheritdoc />
    public string GetContentHash(ProjectModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var envValues = new Dictionary<string, string?>
        {
            [KeystoneProject] = model.ProjectName,
            [KeystoneCoverImage] = model.CoverImage,
            [KeystoneLatexPaperSize] = model.LatexPapersize,
            [KeystoneLatexGeometry] = model.LatexGeometry,
            [KeystoneLatexFontsize] = model.LatexFontsize,
            [KeystoneLatexFontFamily] = model.LatexFontfamily,
            [KeystoneDockerComposeProject] = model.DockerComposeProject,
            [KeystoneDockerImage] = model.DockerImage,
        };

        return contentHashService.ComputeFromKeyValues(envValues);
    }

    /// <summary>
    /// Gets the full path to the <c>.env</c> file in the given project model.
    /// </summary>
    /// <param name="model">The project model.</param>
    /// <returns>The full path to the .env file.</returns>
    private static string GetEnvFilePath(ProjectModel model)
        => Path.Combine(model.ProjectPath, ProjectFiles.EnvFileName);

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
