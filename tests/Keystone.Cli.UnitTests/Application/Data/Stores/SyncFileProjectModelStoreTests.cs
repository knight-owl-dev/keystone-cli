using System.Diagnostics.CodeAnalysis;
using Keystone.Cli.Application.Data.Stores;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Domain.Project;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Data.Stores;

[TestFixture, Parallelizable(ParallelScope.All)]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class SyncFileProjectModelStoreTests
{
    private static SyncFileProjectModelStore Ctor(
        IContentHashService? contentHashService = null,
        IFileSystemService? fileSystemService = null,
        IJsonFileSerializer? jsonFileSerializer = null
    )
        => new(
            contentHashService ?? Substitute.For<IContentHashService>(),
            fileSystemService ?? Substitute.For<IFileSystemService>(),
            jsonFileSerializer ?? Substitute.For<IJsonFileSerializer>()
        );

    [Test]
    public async Task LoadAsync_SyncFileDoesNotExist_ReturnsModelUnchangedAsync()
    {
        const string projectPath = "/test/project";
        var expectedSyncFilePath = Path.Combine(projectPath, ProjectFiles.KeystoneSyncFileName);

        var model = new ProjectModel(projectPath)
        {
            KeystoneSync = new KeystoneSyncModel("existing-template"),
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(expectedSyncFilePath).Returns(false);

        var sut = Ctor(fileSystemService: fileSystemService);

        var actual = await sut.LoadAsync(model);

        Assert.That(actual.KeystoneSync?.TemplateRepositoryName, Is.EqualTo("existing-template"));
    }

    [Test]
    public async Task LoadAsync_SyncFileExists_ReturnsModelWithSyncDataAsync()
    {
        const string projectPath = "/test/project";
        var expectedSyncFilePath = Path.Combine(projectPath, ProjectFiles.KeystoneSyncFileName);

        var model = new ProjectModel(projectPath);

        var syncData = new SyncFileProjectModelStore.SyncFileData
        {
            Source = "keystone",
            Branch = "slim",
            Commit = "6589f5b0f0cd98689946f0398f81aa737d657741",
            Template = "keystone-template-core-slim",
            Timestamp = new DateTime(2025, 6, 22, 17, 47, 47, DateTimeKind.Utc),
            Note = "This file was created by the sync process",
        };

        var expectedKeystoneSync = new KeystoneSyncModel("keystone-template-core-slim")
        {
            SourceRepositoryName = syncData.Source,
            SourceRepositoryBranch = syncData.Branch,
            SourceRepositoryCommit = syncData.Commit,
            TemplateRepositorySyncedAt = syncData.Timestamp,
            Note = syncData.Note,
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(expectedSyncFilePath).Returns(true);

        var jsonFileSerializer = Substitute.For<IJsonFileSerializer>();
        jsonFileSerializer
            .LoadAsync<SyncFileProjectModelStore.SyncFileData>(expectedSyncFilePath, cancellationToken)
            .Returns(syncData);

        var sut = Ctor(fileSystemService: fileSystemService, jsonFileSerializer: jsonFileSerializer);

        var actual = await sut.LoadAsync(model, cancellationToken);

        Assert.That(actual.KeystoneSync, Is.EqualTo(expectedKeystoneSync));
    }

    [Test]
    public async Task LoadAsync_SyncFileExistsWithMissingTemplate_ReturnsModelWithEmptyTemplateAsync()
    {
        const string projectPath = "/test/project";
        var expectedSyncFilePath = Path.Combine(projectPath, ProjectFiles.KeystoneSyncFileName);

        var model = new ProjectModel(projectPath);

        var syncData = new SyncFileProjectModelStore.SyncFileData
        {
            Source = "keystone",
            Branch = "slim",
            Commit = "abc123",
        };

        var expectedKeystoneSync = new KeystoneSyncModel(TemplateRepositoryName: string.Empty)
        {
            SourceRepositoryName = syncData.Source,
            SourceRepositoryBranch = syncData.Branch,
            SourceRepositoryCommit = syncData.Commit,
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(expectedSyncFilePath).Returns(true);

        var jsonFileSerializer = Substitute.For<IJsonFileSerializer>();
        jsonFileSerializer
            .LoadAsync<SyncFileProjectModelStore.SyncFileData>(expectedSyncFilePath)
            .Returns(syncData);

        var sut = Ctor(fileSystemService: fileSystemService, jsonFileSerializer: jsonFileSerializer);

        var actual = await sut.LoadAsync(model);

        Assert.That(actual.KeystoneSync, Is.EqualTo(expectedKeystoneSync));
    }

    [Test]
    public async Task SaveAsync_ModelHasNoKeystoneSync_CompletesImmediatelyAsync()
    {
        const string projectPath = "/test/project";

        var model = new ProjectModel(projectPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        var jsonFileSerializer = Substitute.For<IJsonFileSerializer>();

        var sut = Ctor(fileSystemService: fileSystemService, jsonFileSerializer: jsonFileSerializer);

        await sut.SaveAsync(model);

        await jsonFileSerializer.DidNotReceive().SaveAsync(
            Arg.Any<string>(),
            Arg.Any<SyncFileProjectModelStore.SyncFileData>(),
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public async Task SaveAsync_ModelHasKeystoneSync_SavesSyncDataAsync()
    {
        const string projectPath = "/test/project";
        var expectedSyncFilePath = Path.Combine(projectPath, ProjectFiles.KeystoneSyncFileName);
        var expectedDirectoryPath = Path.Combine(projectPath, ".keystone");

        var keystoneSync = new KeystoneSyncModel("keystone-template-core-slim")
        {
            SourceRepositoryName = "keystone",
            SourceRepositoryBranch = "slim",
            SourceRepositoryCommit = "6589f5b0f0cd98689946f0398f81aa737d657741",
            TemplateRepositorySyncedAt = new DateTime(2025, 6, 22, 17, 47, 47, DateTimeKind.Utc),
            Note = "This file was created by the sync process",
        };

        var syncData = new SyncFileProjectModelStore.SyncFileData
        {
            Source = keystoneSync.SourceRepositoryName,
            Branch = keystoneSync.SourceRepositoryBranch,
            Commit = keystoneSync.SourceRepositoryCommit,
            Template = keystoneSync.TemplateRepositoryName,
            Timestamp = keystoneSync.TemplateRepositorySyncedAt,
            Note = keystoneSync.Note,
        };

        var model = new ProjectModel(projectPath)
        {
            KeystoneSync = keystoneSync,
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var fileSystemService = Substitute.For<IFileSystemService>();
        var jsonFileSerializer = Substitute.For<IJsonFileSerializer>();

        var sut = Ctor(fileSystemService: fileSystemService, jsonFileSerializer: jsonFileSerializer);

        await sut.SaveAsync(model, cancellationToken);

        fileSystemService.Received(1).CreateDirectory(expectedDirectoryPath);
        await jsonFileSerializer.Received(1).SaveAsync(expectedSyncFilePath, syncData, cancellationToken);
    }

    [Test]
    public void GetContentHash_ModelHasNoKeystoneSync_ReturnsHashOfEmptyDictionary()
    {
        const string projectPath = "/test/project";
        const string expectedHash = "empty-hash";

        var model = new ProjectModel(projectPath);

        var contentHashService = Substitute.For<IContentHashService>();
        contentHashService.ComputeFromKeyValues(Arg.Is<Dictionary<string, string?>>(dict => dict.Count == 0))
            .Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }

    [Test]
    public void GetContentHash_ModelHasKeystoneSync_ReturnsHashOfSyncValues()
    {
        const string projectPath = "/test/project";
        const string expectedHash = "sync-hash";
        var timestamp = new DateTime(2025, 6, 22, 17, 47, 47, DateTimeKind.Utc);

        var keystoneSync = new KeystoneSyncModel("keystone-template-core-slim")
        {
            SourceRepositoryName = "keystone",
            SourceRepositoryBranch = "slim",
            SourceRepositoryCommit = "6589f5b0f0cd98689946f0398f81aa737d657741",
            TemplateRepositorySyncedAt = timestamp,
            Note = "This file was created by the sync process",
        };

        var model = new ProjectModel(projectPath)
        {
            KeystoneSync = keystoneSync,
        };

        var contentHashService = Substitute.For<IContentHashService>();
        contentHashService.ComputeFromKeyValues(
            Arg.Is<Dictionary<string, string?>>(dict =>
                dict["source"] == "keystone"
                && dict["branch"] == "slim"
                && dict["commit"] == "6589f5b0f0cd98689946f0398f81aa737d657741"
                && dict["template"] == "keystone-template-core-slim"
                && dict["timestamp"] == timestamp.ToString("O")
                && dict["note"] == "This file was created by the sync process"
            )
        ).Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }
}
