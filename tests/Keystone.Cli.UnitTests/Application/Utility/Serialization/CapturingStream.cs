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
    public byte[]? CapturedBuffer { get; private set; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.CapturedBuffer = ToArray();
        }

        base.Dispose(disposing);
    }
}
