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

        CreateDirectoryTree(destinationPath, entryProvider, predicate, cancellationToken);
        CopyFiles(destinationPath, entryProvider, overwrite, predicate, cancellationToken);
    }

    /// <summary>
    /// Creates the directory tree based on the entries provided by the <paramref name="entryProvider"/>.
    /// </summary>
    /// <param name="destinationPath">The destination root path.</param>
    /// <param name="entryProvider">The entry provider.</param>
    /// <param name="predicate">The entry predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private void CreateDirectoryTree(
        string destinationPath,
        IEntryProvider entryProvider,
        Func<EntryModel, bool>? predicate,
        CancellationToken cancellationToken
    )
    {
        if (! fileSystemService.DirectoryExists(destinationPath))
        {
            logger.LogDebug("Creating directory {DestinationPath}", destinationPath);
            fileSystemService.CreateDirectory(destinationPath);
        }

        var directoryEntries = entryProvider
            .Where(e => e.Type == EntryType.Directory)
            .OrderBy(e => e.RelativePath);

        foreach (var entry in directoryEntries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (predicate?.Invoke(entry) == false)
            {
                logger.LogDebug("Skipping directory based on predicate {RelativePath}", entry.RelativePath);
                continue;
            }

            var destinationEntryPath = entry.GetFullPath(destinationPath);

            if (! fileSystemService.DirectoryExists(destinationEntryPath))
            {
                logger.LogDebug("Creating directory {DestinationEntryPath}", destinationEntryPath);
                _ = fileSystemService.CreateDirectory(destinationEntryPath);
            }
            else
            {
                logger.LogDebug("Directory already exists {DestinationEntryPath}", destinationEntryPath);
            }
        }
    }

    /// <summary>
    /// Copies files from the <paramref name="entryProvider"/> to the specified <paramref name="destinationPath"/>.
    /// </summary>
    /// <param name="destinationPath">The destination root path.</param>
    /// <param name="entryProvider">The entry provider.</param>
    /// <param name="overwrite">Indicates if overwriting existing files is permitted.</param>
    /// <param name="predicate">The entry predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private void CopyFiles(
        string destinationPath,
        IEntryProvider entryProvider,
        bool overwrite,
        Func<EntryModel, bool>? predicate,
        CancellationToken cancellationToken
    )
    {
        var fileEntries = entryProvider
            .Where(e => e.Type == EntryType.File)
            .OrderBy(e => e.RelativePath);

        foreach (var entry in fileEntries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (predicate?.Invoke(entry) == false)
            {
                logger.LogDebug("Skipping file based on predicate {RelativePath}", entry.RelativePath);
                continue;
            }

            var destinationEntryPath = entry.GetFullPath(destinationPath);

            if (! overwrite && fileSystemService.FileExists(destinationEntryPath))
            {
                logger.LogDebug("File already exists, skipping {DestinationEntryPath}", destinationEntryPath);
                continue;
            }

            logger.LogDebug("Writing file {DestinationEntryPath}", destinationEntryPath);
            entryProvider.ExtractToFile(entry, destinationEntryPath);
        }
    }
}
