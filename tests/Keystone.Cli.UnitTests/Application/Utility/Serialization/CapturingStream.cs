using System.Text;


namespace Keystone.Cli.UnitTests.Application.Utility.Serialization;

/// <summary>
/// Used to capture the written bytes when testing serialization
/// because the stream is disposed after writing.
/// </summary>
internal class CapturingStream : MemoryStream
{
    /// <summary>
    /// The captured buffer after the stream is disposed.
    /// </summary>
    private byte[]? CapturedBuffer { get; set; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.CapturedBuffer = ToArray();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Gets the captures string using <see cref="Encoding.UTF8"/> encoding from the captured buffer.
    /// </summary>
    /// <returns>
    /// The captured string, or an empty string if nothing was captured.
    /// </returns>
    public string GetCapturedString()
        => Encoding.UTF8.GetString(this.CapturedBuffer ?? []);
}
