using System.Collections.Immutable;
using Keystone.Cli.Domain.FileSystem;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.FileSystem;

/// <summary>
/// The implementation of <see cref="IFileSystemCopyService"/> for copying files and directories.
/// </summary>
public class FileSystemCopyService(IFileSystemService fileSystemService, ILogger<FileSystemCopyService> logger)
    : IFileSystemCopyService
{
    /// <inheritdoc />
    public void Copy(
        IEntryProvider entryProvider,
        string destinationPath,
        bool overwrite,
        Func<EntryModel, bool>? predicate = null
    )
    {
        ArgumentNullException.ThrowIfNull(entryProvider);
        ArgumentException.ThrowIfNullOrEmpty(destinationPath);

        if (! fileSystemService.DirectoryExists(destinationPath))
        {
            logger.LogDebug("Creating directory {DestinationPath}", destinationPath);
            fileSystemService.CreateDirectory(destinationPath);
        }

        var entries = EntryNode.CreateNodes(entryProvider).SelectMany(node
            => node.Aggregate(
                ImmutableList.CreateBuilder<EntryModel>(),
                predicate ?? EntryModelPredicates.AcceptAll,
                (acc, entry) =>
                {
                    acc.Add(entry);
                    return acc;
                },
                acc => acc.ToImmutable()
            )
        );

        foreach (var entry in entries)
        {
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
