using System.IO.Compression;
using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.Application.GitHub;

/// <summary>
/// The GitHub zip entry provider factory.
/// </summary>
public class GitHubZipEntryProviderFactory(IHttpClientFactory httpClientFactory)
    : IGitHubZipEntryProviderFactory
{
    private const string HttpClientName = "GitHub";

    /// <inheritdoc />
    public async Task<IEntryProvider> CreateAsync(Uri repositoryUrl, string branchName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(repositoryUrl);
        ArgumentException.ThrowIfNullOrEmpty(branchName);

        var zipUrl = GetZipUrl(repositoryUrl, branchName);
        var httpClient = httpClientFactory.CreateClient(HttpClientName);

        var zipStream = await httpClient
            .GetStreamAsync(zipUrl, cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
            try
            {
                return new GitHubZipEntryProvider(ZipFileExtensions.ExtractToFile, archive);
            }
            catch
            {
                await archive.DisposeAsync();
                throw;
            }
        }
        catch
        {
            await zipStream.DisposeAsync();
            throw;
        }
    }

    private static Uri GetZipUrl(Uri repositoryUrl, string branchName)
        => new($"{repositoryUrl}/archive/refs/heads/{branchName}.zip");
}
