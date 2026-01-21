using System.Text;


namespace Keystone.Cli.UnitTests.Application.Utility.Serialization;

/// <summary>
/// Used to capture the written bytes when testing serialization
/// because the stream is disposed after writing.
/// </summary>
internal sealed class CapturingStream : MemoryStream
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
    /// Sets the new content of the stream using <see cref="Encoding.UTF8"/> encoding.
    /// </summary>
    /// <remarks>
    /// This will reset the position to the beginning of the stream, reset the length,
    /// and clear any previously captured buffer.
    /// </remarks>
    /// <param name="content">The new content.</param>
    public CapturingStream SetContent(string content)
    {
        this.CapturedBuffer = null;
        Write(Encoding.UTF8.GetBytes(content));

        SetLength(this.Position);
        Seek(0, SeekOrigin.Begin);

        return this;
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
