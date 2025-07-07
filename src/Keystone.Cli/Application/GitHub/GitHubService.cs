using Keystone.Cli.Application.Utility;
using Keystone.Cli.Domain.FileSystem;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.GitHub;

/// <summary>
/// Implementation of <see cref="IGitHubService"/> for basic GitHub operations.
/// </summary>
public class GitHubService(
    IFileSystemService fileSystemService,
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

        logger.LogDebug(
            "Downloading repository {RepositoryUrl} branch {BranchName} to {DestinationPath}",
            repositoryUrl,
            branchName,
            destinationPath
        );

        using var entryProvider = await gitHubZipEntryProviderFactory
            .CreateAsync(repositoryUrl, branchName, cancellationToken)
            .ConfigureAwait(false);

        if (! fileSystemService.DirectoryExists(destinationPath))
        {
            logger.LogDebug("Creating directory {DestinationPath}", destinationPath);
            fileSystemService.CreateDirectory(destinationPath);
        }

        foreach (var entry in entryProvider)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (predicate?.Invoke(entry) == false)
            {
                logger.LogDebug("Skipping {EntryType} based on predicate {RelativePath}", entry.Type, entry.RelativePath);
                continue;
            }

            var destinationEntryPath = entry.GetFullPath(destinationPath);

            switch (entry.Type)
            {
                case EntryType.File when ! overwrite && fileSystemService.FileExists(destinationEntryPath):
                    logger.LogDebug("File already exists, skipping {DestinationEntryPath}", destinationEntryPath);
                    continue;

                case EntryType.File:
                    logger.LogDebug("Writing file {DestinationEntryPath}", destinationEntryPath);
                    entryProvider.ExtractToFile(entry, destinationEntryPath);
                    continue;

                case EntryType.Directory when fileSystemService.DirectoryExists(destinationEntryPath):
                    logger.LogDebug("Directory already exists {DestinationEntryPath}", destinationEntryPath);
                    continue;

                case EntryType.Directory:
                    logger.LogDebug("Creating directory {DestinationEntryPath}", destinationEntryPath);
                    _ = fileSystemService.CreateDirectory(destinationEntryPath);
                    continue;

                default:
                    logger.LogWarning("Unknown entry type {EntryType} for {RelativePath}", entry.Type, entry.RelativePath);
                    continue;
            }
        }
    }
}
