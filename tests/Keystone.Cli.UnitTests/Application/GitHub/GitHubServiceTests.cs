using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.GitHub;
using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.GitHub;

[TestFixture, Parallelizable(ParallelScope.All)]
public class GitHubServiceTests
{
    private static GitHubService Ctor(
        IFileSystemCopyService? fileSystemCopyService = null,
        IGitHubZipEntryCollectionFactory? gitHubZipEntryCollectionFactory = null,
        ILogger<GitHubService>? logger = null
    )
        => new(
            fileSystemCopyService ?? Substitute.For<IFileSystemCopyService>(),
            gitHubZipEntryCollectionFactory ?? Substitute.For<IGitHubZipEntryCollectionFactory>(),
            logger ?? new TestLogger<GitHubService>()
        );

    [Test]
    public async Task CopyPublicRepositoryAsync_RepositoryUrlIsNull_ThrowsArgumentNullExceptionAsync()
    {
        var sut = Ctor();

        await Assert.ThatAsync(
            () => sut.CopyPublicRepositoryAsync(
                repositoryUrl: null!,
                branchName: "main",
                destinationPath: ".",
                overwrite: false
            ),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("repositoryUrl")
        );
    }

    [Test]
    public async Task CopyPublicRepositoryAsync_BranchNameIsNull_ThrowsArgumentNullExceptionAsync()
    {
        var sut = Ctor();

        await Assert.ThatAsync(
            () => sut.CopyPublicRepositoryAsync(
                repositoryUrl: new Uri("https://abc.com/abc/def"),
                branchName: null!,
                destinationPath: ".",
                overwrite: false
            ),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("branchName")
        );
    }

    [Test]
    public async Task CopyPublicRepositoryAsync_BranchNameIsEmpty_ThrowsArgumentExceptionAsync()
    {
        var sut = Ctor();

        await Assert.ThatAsync(
            () => sut.CopyPublicRepositoryAsync(
                repositoryUrl: new Uri("https://abc.com/abc/def"),
                branchName: string.Empty,
                destinationPath: ".",
                overwrite: false
            ),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("branchName")
        );
    }

    [Test]
    public async Task CopyPublicRepositoryAsync_DestinationPathIsNull_ThrowsArgumentNullExceptionAsync()
    {
        var sut = Ctor();

        await Assert.ThatAsync(
            () => sut.CopyPublicRepositoryAsync(
                repositoryUrl: new Uri("https://abc.com/abc/def"),
                branchName: "main",
                destinationPath: null!,
                overwrite: false
            ),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("destinationPath")
        );
    }

    [Test]
    public async Task CopyPublicRepositoryAsync_DestinationPathIsEmpty_ThrowsArgumentExceptionAsync()
    {
        var sut = Ctor();

        await Assert.ThatAsync(
            () => sut.CopyPublicRepositoryAsync(
                repositoryUrl: new Uri("https://abc.com/abc/def"),
                branchName: "main",
                destinationPath: string.Empty,
                overwrite: false
            ),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("destinationPath")
        );
    }

    [Test]
    public async Task CopyPublicRepositoryAsync_ValidParameters_CallsCopyMethodAsync()
    {
        var repositoryUrl = new Uri("https://abc.com/abc/def");
        const string branchName = "main";
        const string destinationPath = "test/path";

        var fileSystemCopyService = Substitute.For<IFileSystemCopyService>();
        var gitHubZipEntryCollectionFactory = Substitute.For<IGitHubZipEntryCollectionFactory>();
        var entryCollection = Substitute.For<IEntryCollection>();

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        gitHubZipEntryCollectionFactory
            .CreateAsync(repositoryUrl, branchName, cancellationToken)
            .Returns(Task.FromResult(entryCollection));

        var sut = Ctor(
            fileSystemCopyService: fileSystemCopyService,
            gitHubZipEntryCollectionFactory: gitHubZipEntryCollectionFactory
        );

        await sut.CopyPublicRepositoryAsync(
            repositoryUrl,
            branchName,
            destinationPath,
            overwrite: false,
            cancellationToken: cancellationToken
        );

        fileSystemCopyService.Received(1).Copy(
            entryCollection,
            destinationPath,
            overwrite: false
        );
    }

    [Test]
    public async Task CopyPublicRepositoryAsync_DisposesEntryProviderAsync()
    {
        var repositoryUrl = new Uri("https://abc.com/abc/def");
        const string branchName = "main";
        const string destinationPath = "test/path";

        var fileSystemCopyService = Substitute.For<IFileSystemCopyService>();
        var gitHubZipEntryCollectionFactory = Substitute.For<IGitHubZipEntryCollectionFactory>();
        var entryCollection = Substitute.For<IEntryCollection>();

        gitHubZipEntryCollectionFactory
            .CreateAsync(repositoryUrl, branchName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(entryCollection));

        var sut = Ctor(
            fileSystemCopyService: fileSystemCopyService,
            gitHubZipEntryCollectionFactory: gitHubZipEntryCollectionFactory
        );

        await sut.CopyPublicRepositoryAsync(
            repositoryUrl,
            branchName,
            destinationPath,
            overwrite: false
        );

        entryCollection.Received(1).Dispose();
    }

    [Test]
    public async Task CopyPublicRepositoryAsync_HonorsOverwriteAsync()
    {
        var repositoryUrl = new Uri("https://abc.com/abc/def");
        const string branchName = "main";
        const string destinationPath = "test/path";

        var fileSystemCopyService = Substitute.For<IFileSystemCopyService>();
        var gitHubZipEntryCollectionFactory = Substitute.For<IGitHubZipEntryCollectionFactory>();
        var entryCollection = Substitute.For<IEntryCollection>();

        gitHubZipEntryCollectionFactory
            .CreateAsync(repositoryUrl, branchName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(entryCollection));

        var sut = Ctor(
            fileSystemCopyService: fileSystemCopyService,
            gitHubZipEntryCollectionFactory: gitHubZipEntryCollectionFactory
        );

        await sut.CopyPublicRepositoryAsync(
            repositoryUrl,
            branchName,
            destinationPath,
            overwrite: true
        );

        fileSystemCopyService.Received(1).Copy(
            entryCollection,
            destinationPath,
            overwrite: true
        );
    }

    [Test]
    public async Task CopyPublicRepositoryAsync_HonorsPredicateAsync()
    {
        var repositoryUrl = new Uri("https://abc.com/abc/def");
        const string branchName = "main";
        const string destinationPath = "test/path";

        var fileSystemCopyService = Substitute.For<IFileSystemCopyService>();
        var gitHubZipEntryCollectionFactory = Substitute.For<IGitHubZipEntryCollectionFactory>();
        var entryCollection = Substitute.For<IEntryCollection>();
        var predicate = Substitute.For<Func<EntryModel, bool>>();

        gitHubZipEntryCollectionFactory
            .CreateAsync(repositoryUrl, branchName, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(entryCollection));

        var sut = Ctor(
            fileSystemCopyService: fileSystemCopyService,
            gitHubZipEntryCollectionFactory: gitHubZipEntryCollectionFactory
        );

        await sut.CopyPublicRepositoryAsync(
            repositoryUrl,
            branchName,
            destinationPath,
            overwrite: false,
            predicate: predicate
        );

        fileSystemCopyService.Received(1).Copy(
            entryCollection,
            destinationPath,
            overwrite: false,
            predicate
        );
    }
}
