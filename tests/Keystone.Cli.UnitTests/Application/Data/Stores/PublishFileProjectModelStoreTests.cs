using Keystone.Cli.Application.Data.Stores;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Domain.Project;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Data.Stores;

[TestFixture, Parallelizable(ParallelScope.All)]
public class PublishFileProjectModelStoreTests
{
    private static PublishFileProjectModelStore Ctor(
        IContentHashService? contentHashService = null,
        IFileSystemService? fileSystemService = null,
        ITextFileSerializer? textFileSerializer = null
    )
        => new(
            contentHashService ?? Substitute.For<IContentHashService>(),
            fileSystemService ?? Substitute.For<IFileSystemService>(),
            textFileSerializer ?? Substitute.For<ITextFileSerializer>()
        );

    [Test]
    public async Task LoadAsync_PublishFileDoesNotExist_ReturnsModelUnchangedAsync()
    {
        const string projectPath = "/test/project";
        var model = new ProjectModel(projectPath);

        var sut = Ctor();

        var actual = await sut.LoadAsync(model);

        Assert.That(actual, Is.SameAs(model));
    }

    [Test]
    public async Task LoadAsync_PublishFileExists_ReturnsModelWithContentFilePathsAsync()
    {
        const string projectPath = "/test/project";
        var publishFilePath = Path.Combine(projectPath, "publish.txt");
        var model = new ProjectModel(projectPath);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        string[] expectedPaths = ["chapters/intro.md", "chapters/chapter1.md", "appendix/appendix-a.md"];

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(publishFilePath).Returns(true);

        var textFileSerializer = Substitute.For<ITextFileSerializer>();
        textFileSerializer.LoadLinesAsync(publishFilePath, cancellationToken).Returns(expectedPaths);

        var sut = Ctor(fileSystemService: fileSystemService, textFileSerializer: textFileSerializer);

        var actual = await sut.LoadAsync(model, cancellationToken);

        Assert.That(actual.ContentFilePaths, Is.EqualTo(expectedPaths));
    }

    [Test]
    public async Task LoadAsync_EmptyPublishFile_ReturnsModelWithEmptyContentFilePathsAsync()
    {
        const string projectPath = "/test/project";
        var publishFilePath = Path.Combine(projectPath, "publish.txt");
        var model = new ProjectModel(projectPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(publishFilePath).Returns(true);

        var textFileSerializer = Substitute.For<ITextFileSerializer>();
        textFileSerializer.LoadLinesAsync(publishFilePath).Returns([]);

        var sut = Ctor(fileSystemService: fileSystemService, textFileSerializer: textFileSerializer);

        var result = await sut.LoadAsync(model);

        Assert.That(result.ContentFilePaths, Is.EqualTo(Array.Empty<string>()));
    }

    [Test]
    public async Task SaveAsync_ContentFilePaths_IsNull_SavesEmptyLinesToPublishFileAsync()
    {
        const string projectPath = "/test/project";
        var publishFilePath = Path.Combine(projectPath, "publish.txt");

        var model = new ProjectModel(projectPath)
        {
            ContentFilePaths = null,
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var textFileSerializer = Substitute.For<ITextFileSerializer>();
        var sut = Ctor(textFileSerializer: textFileSerializer);

        await sut.SaveAsync(model, cancellationToken);

        await textFileSerializer.Received(1).SaveLinesAsync(
            publishFilePath,
            Arg.Is<IEnumerable<string>>(lines => ! lines.Any()),
            cancellationToken
        );
    }

    [Test]
    public async Task SaveAsync_HasContentFilePaths_SavesLinesToPublishFileAsync()
    {
        const string projectPath = "/test/project";
        var publishFilePath = Path.Combine(projectPath, "publish.txt");
        string[] contentFilePaths = ["chapters/intro.md", "chapters/chapter1.md"];

        var model = new ProjectModel(projectPath)
        {
            ContentFilePaths = contentFilePaths,
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var textFileSerializer = Substitute.For<ITextFileSerializer>();
        var sut = Ctor(textFileSerializer: textFileSerializer);

        await sut.SaveAsync(model, cancellationToken);

        await textFileSerializer.Received(1).SaveLinesAsync(
            publishFilePath,
            Arg.Is<IEnumerable<string>>(lines => lines.SequenceEqual(contentFilePaths)),
            cancellationToken
        );
    }

    [Test]
    public void GetContentHash_ContentFilePathsIsNull_ReturnsHashOfEmptyLines()
    {
        const string expectedHash = "empty-hash";
        const string projectPath = "/test/project";

        var model = new ProjectModel(projectPath)
        {
            ContentFilePaths = null,
        };

        var contentHashService = Substitute.For<IContentHashService>();

        contentHashService
            .ComputeFromLines(Arg.Is<IEnumerable<string>>(lines => ! lines.Any()))
            .Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }

    [Test]
    public void GetContentHash_HasContentFilePaths_ReturnsHashOfLines()
    {
        const string expectedHash = "hash-of-content";
        const string projectPath = "/test/project";
        string[] contentFilePaths = ["chapters/intro.md", "chapters/chapter1.md"];

        var model = new ProjectModel(projectPath)
        {
            ContentFilePaths = contentFilePaths,
        };

        var contentHashService = Substitute.For<IContentHashService>();

        contentHashService
            .ComputeFromLines(Arg.Is<IEnumerable<string>>(lines => lines.SequenceEqual(contentFilePaths)))
            .Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var result = sut.GetContentHash(model);

        Assert.That(result, Is.EqualTo(expectedHash));
    }
}
