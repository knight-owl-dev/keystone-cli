using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Domain.FileSystem;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.GitHub;

/// <summary>
/// Implementation of <see cref="IGitHubService"/> for basic GitHub operations.
/// </summary>
public partial class GitHubService(
    IFileSystemCopyService fileSystemCopyService,
    IGitHubZipEntryProviderFactory gitHubZipEntryProviderFactory,
    ILogger<GitHubService> logger
)
    : IGitHubService
{
    /// <inheritdoc />
    public async Task CopyPublicRepositoryAsync(
        Uri repositoryUrl,
        string branchName,
        string destinationPath,
        bool overwrite,
        Func<EntryModel, bool>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(repositoryUrl);
        ArgumentException.ThrowIfNullOrEmpty(branchName);
        ArgumentException.ThrowIfNullOrEmpty(destinationPath);

        LogDownloadingRepository(logger, repositoryUrl, branchName, destinationPath);

        using var entryProvider = await gitHubZipEntryProviderFactory
            .CreateAsync(repositoryUrl, branchName, cancellationToken)
            .ConfigureAwait(false);

        fileSystemCopyService.Copy(
            entryProvider,
            destinationPath,
            overwrite,
            predicate
        );
    }

    [LoggerMessage(LogLevel.Debug, "Downloading repository {RepositoryUrl} branch {BranchName} to {DestinationPath}")]
    static partial void LogDownloadingRepository(
        ILogger<GitHubService> logger,
        Uri repositoryUrl,
        string branchName,
        string destinationPath
    );
}
