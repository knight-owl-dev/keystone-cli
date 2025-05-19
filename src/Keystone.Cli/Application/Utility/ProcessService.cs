using System.Diagnostics;


namespace Keystone.Cli.Application.Utility;

/// <summary>
/// The process service implementation.
/// </summary>
public class ProcessService
    : IProcessService
{
    /// <inheritdoc />
    public void OpenBrowser(Uri url)
    {
        Process.Start(
            new ProcessStartInfo(url.AbsoluteUri)
            {
                UseShellExecute = true,
            }
        );
    }
}
