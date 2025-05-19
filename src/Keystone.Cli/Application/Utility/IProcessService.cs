namespace Keystone.Cli.Application.Utility;

/// <summary>
/// The process service.
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// Opens a browser with the specified URL.
    /// </summary>
    /// <param name="url">The target URL to open.</param>
    void OpenBrowser(Uri url);
}
