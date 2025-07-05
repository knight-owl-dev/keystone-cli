using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.Application.Utility;

/// <summary>
/// Basic interface for GitHub service.
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// Copy source code from a public GitHub repository to a local directory.
    /// </summary>
    /// <param name="repositoryUrl">The source repository URL.</param>
    /// <param name="branchName">The branch name.</param>
    /// <param name="destinationPath">The destination path for the copied source code.</param>
    /// <param name="overwrite">Indicates if overwriting existing files.</param>
    /// <param name="predicate">
    /// If provided, the file system entries predicate implementing acceptance criteria for file paths relative to the repository root.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="repositoryUrl"/>, <paramref name="branchName"/> or <paramref name="destinationPath"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="branchName"/> or <paramref name="destinationPath"/> is empty or contains only whitespace.
    /// </exception>
    Task CopyPublicRepositoryAsync(
        Uri repositoryUrl,
        string branchName,
        string destinationPath,
        bool overwrite,
        Func<EntryModel, bool>? predicate = null,
        CancellationToken cancellationToken = default
    );
}
