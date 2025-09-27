using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Domain.Project;
using System.Text.Json.Serialization;


namespace Keystone.Cli.Application.Data.Stores;

/// <summary>
/// An implementation of <see cref="IProjectModelStore"/> that binds <see cref="ProjectModel.KeystoneSync"/>
/// to <see cref="ProjectFiles.KeystoneSyncFileName"/> contents.
/// </summary>
public class SyncFileProjectModelStore(
    IContentHashService contentHashService,
    IFileSystemService fileSystemService,
    IJsonFileSerializer jsonFileSerializer
)
    : IProjectModelStore
{
    private const string KeySource = "source";
    private const string KeyBranch = "branch";
    private const string KeyCommit = "commit";
    private const string KeyTemplate = "template";
    private const string KeyTimestamp = "timestamp";
    private const string KeyNote = "note";

    /// <inheritdoc />
    public async Task<ProjectModel> LoadAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        var syncFilePath = GetSyncFilePath(model);

        if (! fileSystemService.FileExists(syncFilePath))
        {
            return model;
        }

        var syncData = await jsonFileSerializer.LoadAsync<SyncFileData>(syncFilePath, cancellationToken);

        var keystoneSync = new KeystoneSyncModel(syncData.Template ?? string.Empty)
        {
            SourceRepositoryName = syncData.Source,
            SourceRepositoryBranch = syncData.Branch,
            SourceRepositoryCommit = syncData.Commit,
            TemplateRepositorySyncedAt = syncData.Timestamp,
            Note = syncData.Note,
        };

        return model with
        {
            KeystoneSync = keystoneSync,
        };
    }

    /// <inheritdoc />
    public Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        if (model.KeystoneSync is null)
        {
            return Task.CompletedTask;
        }

        var syncFilePath = GetSyncFilePath(model);

        var syncFileDirectoryName = Path.GetDirectoryName(syncFilePath)!;
        fileSystemService.CreateDirectory(syncFileDirectoryName);

        var syncData = new SyncFileData
        {
            Source = model.KeystoneSync.SourceRepositoryName,
            Branch = model.KeystoneSync.SourceRepositoryBranch,
            Commit = model.KeystoneSync.SourceRepositoryCommit,
            Template = model.KeystoneSync.TemplateRepositoryName,
            Timestamp = model.KeystoneSync.TemplateRepositorySyncedAt,
            Note = model.KeystoneSync.Note,
        };

        return jsonFileSerializer.SaveAsync(syncFilePath, syncData, cancellationToken);
    }

    /// <inheritdoc />
    public string GetContentHash(ProjectModel model)
    {
        if (model.KeystoneSync is null)
        {
            return contentHashService.ComputeFromKeyValues(new Dictionary<string, string?>());
        }

        var syncValues = new Dictionary<string, string?>
        {
            [KeySource] = model.KeystoneSync.SourceRepositoryName,
            [KeyBranch] = model.KeystoneSync.SourceRepositoryBranch,
            [KeyCommit] = model.KeystoneSync.SourceRepositoryCommit,
            [KeyTemplate] = model.KeystoneSync.TemplateRepositoryName,
            [KeyTimestamp] = model.KeystoneSync.TemplateRepositorySyncedAt?.ToString("O"),
            [KeyNote] = model.KeystoneSync.Note,
        };

        return contentHashService.ComputeFromKeyValues(syncValues);
    }

    private static string GetSyncFilePath(ProjectModel model)
        => Path.Combine(model.ProjectPath, ProjectFiles.KeystoneSyncFileName);

    /// <summary>
    /// Internal data contract for serializing/deserializing the <c>sync.json.</c> file.
    /// </summary>
    public record SyncFileData
    {
        [JsonPropertyName(KeySource)]
        public string? Source { get; init; }

        [JsonPropertyName(KeyBranch)]
        public string? Branch { get; init; }

        [JsonPropertyName(KeyCommit)]
        public string? Commit { get; init; }

        [JsonPropertyName(KeyTemplate)]
        public string? Template { get; init; }

        [JsonPropertyName(KeyTimestamp)]
        public DateTime? Timestamp { get; init; }

        [JsonPropertyName(KeyNote)]
        public string? Note { get; init; }
    }
}
