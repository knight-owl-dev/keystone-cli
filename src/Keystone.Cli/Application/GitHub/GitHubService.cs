using System.IO.Compression;
using Keystone.Cli.Domain.FileSystem;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.GitHub;

/// <summary>
/// Implementation of <see cref="IGitHubService"/> for basic GitHub operations.
/// </summary>
public class GitHubService(IHttpClientFactory httpClientFactory, ILogger<GitHubService> logger)
    : IGitHubService
{
    private const string HttpClientName = "GitHub";

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

        var zipUrl = GetZipUrl(repositoryUrl, branchName);
        using var httpClient = httpClientFactory.CreateClient(HttpClientName);
        await using var zipStream = await httpClient.GetStreamAsync(zipUrl, cancellationToken);

        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);
        CopyFiles(archive, destinationPath, overwrite, predicate);
    }

    private void CopyFiles(ZipArchive archive, string destinationPath, bool overwrite, Func<EntryModel, bool>? predicate)
    {
        // GitHub zips wrap content in a top-level folder
        var rootArchiveEntry = archive.Entries[0];

        if (! Directory.Exists(destinationPath))
        {
            logger.LogDebug("Creating directory {DestinationPath}", destinationPath);
            Directory.CreateDirectory(destinationPath);
        }

        foreach (var archiveEntry in archive.Entries.Skip(1))
        {
            var entryModel = new EntryModel(
                GetEntryType(archiveEntry),
                Path.GetFileName(archiveEntry.Name),
                MakeRelative(rootArchiveEntry, archiveEntry.FullName)
            );

            if (predicate?.Invoke(entryModel) == false)
            {
                logger.LogDebug("Skipping {EntryType} {EntryName} based on predicate", entryModel.Type, archiveEntry.FullName);
                continue;
            }

            var destinationEntryPath = entryModel.GetFullPath(destinationPath);
            switch (entryModel.Type)
            {
                case EntryType.File:
                    logger.LogDebug("Writing file {DestinationEntryPath}", destinationEntryPath);
                    archiveEntry.ExtractToFile(destinationEntryPath, overwrite);
                    break;

                case EntryType.Directory when ! Directory.Exists(destinationEntryPath):
                    logger.LogDebug("Creating directory {DestinationEntryPath}", destinationEntryPath);
                    Directory.CreateDirectory(destinationEntryPath);
                    break;

                default:
                    logger.LogDebug("Directory {DestinationEntryPath} already exists", destinationEntryPath);
                    break;
            }
        }
    }

    private static string MakeRelative(ZipArchiveEntry rootEntry, string path)
        => path.StartsWith(rootEntry.FullName) ? path[rootEntry.FullName.Length..] : path;

    private static EntryType GetEntryType(ZipArchiveEntry entry)
        => entry.FullName.EndsWith('/') ? EntryType.Directory : EntryType.File;

    private static Uri GetZipUrl(Uri repositoryUrl, string branchName)
        => new($"{repositoryUrl}/archive/refs/heads/{branchName}.zip");
}
