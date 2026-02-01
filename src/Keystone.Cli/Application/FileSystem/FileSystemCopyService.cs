using System.Collections.Immutable;
using Keystone.Cli.Domain.FileSystem;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.Application.FileSystem;

/// <summary>
/// The implementation of <see cref="IFileSystemCopyService"/> for copying files and directories.
/// </summary>
public partial class FileSystemCopyService(IFileSystemService fileSystemService, ILogger<FileSystemCopyService> logger)
    : IFileSystemCopyService
{
    /// <inheritdoc />
    public void Copy(
        IEntryCollection entryCollection,
        string destinationPath,
        bool overwrite,
        Func<EntryModel, bool>? predicate = null
    )
    {
        ArgumentNullException.ThrowIfNull(entryCollection);
        ArgumentException.ThrowIfNullOrEmpty(destinationPath);

        if (!fileSystemService.DirectoryExists(destinationPath))
        {
            LogCreatingDirectory(logger, destinationPath);
            fileSystemService.CreateDirectory(destinationPath);
        }

        var entries = EntryNode.CreateNodes(entryCollection).SelectMany(node
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
                case EntryType.File when !overwrite && fileSystemService.FileExists(destinationEntryPath):
                    LogFileAlreadyExists(logger, destinationEntryPath);
                    continue;

                case EntryType.File:
                    LogWritingFile(logger, destinationEntryPath);
                    entryCollection.ExtractToFile(entry, destinationEntryPath);
                    continue;

                case EntryType.Directory when fileSystemService.DirectoryExists(destinationEntryPath):
                    LogDirectoryAlreadyExists(logger, destinationEntryPath);
                    continue;

                case EntryType.Directory:
                    LogCreatingDirectory(logger, destinationEntryPath);
                    _ = fileSystemService.CreateDirectory(destinationEntryPath);
                    continue;

                default:
                    LogUnknownEntryType(logger, entry.Type, entry.RelativePath);
                    continue;
            }
        }
    }

    [LoggerMessage(LogLevel.Debug, "Creating directory {DestinationPath}")]
    static partial void LogCreatingDirectory(ILogger<FileSystemCopyService> logger, string destinationPath);

    [LoggerMessage(LogLevel.Debug, "File already exists, skipping {DestinationEntryPath}")]
    static partial void LogFileAlreadyExists(ILogger<FileSystemCopyService> logger, string destinationEntryPath);

    [LoggerMessage(LogLevel.Debug, "Writing file {DestinationEntryPath}")]
    static partial void LogWritingFile(ILogger<FileSystemCopyService> logger, string destinationEntryPath);

    [LoggerMessage(LogLevel.Debug, "Directory already exists {DestinationEntryPath}")]
    static partial void LogDirectoryAlreadyExists(ILogger<FileSystemCopyService> logger, string destinationEntryPath);

    [LoggerMessage(LogLevel.Warning, "Unknown entry type {EntryType} for {RelativePath}")]
    static partial void LogUnknownEntryType(ILogger<FileSystemCopyService> logger, EntryType entryType, string relativePath);
}
