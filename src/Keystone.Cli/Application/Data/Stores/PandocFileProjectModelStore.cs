using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Application.Utility.Serialization.Yaml;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Data.Stores;

/// <summary>
/// An implementation of <see cref="IProjectModelStore"/> that binds various <see cref="ProjectModel"/> properties
/// to <see cref="ProjectFiles.PandocFileName"/> file contents.
/// </summary>
public class PandocFileProjectModelStore(
    IContentHashService contentHashService,
    IFileSystemService fileSystemService,
    IYamlFileSerializer yamlFileSerializer
)
    : IProjectModelStore
{
    private const string KeyTitle = "title";
    private const string KeySubtitle = "subtitle";
    private const string KeyAuthor = "author";
    private const string KeyDate = "date";
    private const string KeyLang = "lang";
    private const string KeyFooterCopyright = "footer-copyright";
    private const string KeyDescription = "description";
    private const string KeyKeywords = "keywords";

    /// <inheritdoc />
    public async Task<ProjectModel> LoadAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        var pandocFilePath = GetPandocFilePath(model);

        if (! fileSystemService.FileExists(pandocFilePath))
        {
            return model;
        }

        var yamlData = await yamlFileSerializer.LoadAsync(pandocFilePath, cancellationToken);

        return model with
        {
            Title = YamlSerializationHelpers.GetScalarValueOrDefault(yamlData, KeyTitle),
            Subtitle = YamlSerializationHelpers.GetScalarValueOrDefault(yamlData, KeySubtitle),
            Author = YamlSerializationHelpers.GetScalarValueOrDefault(yamlData, KeyAuthor),
            Date = YamlSerializationHelpers.GetScalarValueOrDefault(yamlData, KeyDate),
            Lang = YamlSerializationHelpers.GetScalarValueOrDefault(yamlData, KeyLang),
            FooterCopyright = YamlSerializationHelpers.GetScalarValueOrDefault(yamlData, KeyFooterCopyright),
            Description = YamlSerializationHelpers.GetScalarValueOrDefault(yamlData, KeyDescription),
            Keywords = YamlSerializationHelpers.GetArrayValueOrDefault(yamlData, KeyKeywords),
        };
    }

    /// <inheritdoc />
    public Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        var pandocFilePath = GetPandocFilePath(model);

        var yamlData = new Dictionary<string, YamlValue>
        {
            [KeyTitle] = YamlSerializationHelpers.CreateYamlScalar(model.Title),
            [KeySubtitle] = YamlSerializationHelpers.CreateYamlScalar(model.Subtitle),
            [KeyAuthor] = YamlSerializationHelpers.CreateYamlScalar(model.Author),
            [KeyDate] = YamlSerializationHelpers.CreateYamlScalar(model.Date),
            [KeyLang] = YamlSerializationHelpers.CreateYamlScalar(model.Lang),
            [KeyFooterCopyright] = YamlSerializationHelpers.CreateYamlScalar(model.FooterCopyright),
            [KeyDescription] = YamlSerializationHelpers.CreateYamlScalar(model.Description),
            [KeyKeywords] = YamlSerializationHelpers.CreateYamlArray(model.Keywords),
        };

        return yamlFileSerializer.SaveAsync(pandocFilePath, yamlData, cancellationToken);
    }

    /// <inheritdoc />
    public string GetContentHash(ProjectModel model)
    {
        var pandocValues = new Dictionary<string, string?>
        {
            [KeyTitle] = model.Title,
            [KeySubtitle] = model.Subtitle,
            [KeyAuthor] = model.Author,
            [KeyDate] = model.Date,
            [KeyLang] = model.Lang,
            [KeyFooterCopyright] = model.FooterCopyright,
            [KeyDescription] = model.Description,
            [KeyKeywords] = model.Keywords != null ? string.Join(",", model.Keywords) : null,
        };

        return contentHashService.ComputeFromKeyValues(pandocValues);
    }

    private static string GetPandocFilePath(ProjectModel model)
        => Path.Combine(model.ProjectPath, ProjectFiles.PandocFileName);
}
