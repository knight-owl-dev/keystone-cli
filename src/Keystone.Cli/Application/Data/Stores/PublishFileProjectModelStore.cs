using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Data.Stores;

/// <summary>
/// An implementation of <see cref="IProjectModelStore"/> that binds <see cref="ProjectModel.ContentFilePaths"/>
/// to <see cref="ProjectFiles.PublishFileName"/> contents.
/// </summary>
public class PublishFileProjectModelStore(
    IContentHashService contentHashService,
    IFileSystemService fileSystemService,
    ITextFileSerializer textFileSerializer
)
    : IProjectModelStore
{
    /// <inheritdoc />
    public async Task<ProjectModel> LoadAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        var publishFilePath = GetPublishFilePath(model);

        if (! fileSystemService.FileExists(publishFilePath))
        {
            return model;
        }

        return model with
        {
            ContentFilePaths = [..await textFileSerializer.LoadLinesAsync(publishFilePath, cancellationToken)],
        };
    }

    /// <inheritdoc />
    public Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default)
        => textFileSerializer.SaveLinesAsync(GetPublishFilePath(model), model.ContentFilePaths ?? [], cancellationToken);

    /// <inheritdoc />
    public string GetContentHash(ProjectModel model)
        => contentHashService.ComputeFromLines(model.ContentFilePaths ?? []);

    private static string GetPublishFilePath(ProjectModel model)
        => Path.Combine(model.ProjectPath, ProjectFiles.PublishFileName);
}
