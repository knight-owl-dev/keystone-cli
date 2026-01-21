using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.Application.GitHub;

/// <summary>
/// Defines a factory for creating GitHub zip entry collections.
/// </summary>
public interface IGitHubZipEntryCollectionFactory
{
    /// <summary>
    /// Creates a new <see cref="IEntryCollection"/> instance that provides access to entries from the specified GitHub repository branch.
    /// </summary>
    /// <remarks>
    /// This method downloads the repository as a zip archive and provides an entry provider for accessing its contents.
    /// The caller must dispose the returned <see cref="IEntryCollection"/> when done to release resources.
    /// </remarks>
    /// <param name="repositoryUrl">The URL of the GitHub repository.</param>
    /// <param name="branchName">The name of the branch to download.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the created <see cref="IEntryCollection"/>.
    /// </returns>
    Task<IEntryCollection> CreateAsync(Uri repositoryUrl, string branchName, CancellationToken cancellationToken = default);
}
