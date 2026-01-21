using System.Text;
using Keystone.Cli.Application.Utility.Text;


namespace Keystone.Cli.UnitTests.Application.Utility.Text;

[TestFixture, Parallelizable(ParallelScope.All)]
public class TextFileUtilityTests
{
    [Test]
    public async Task ReadLinesAsync_ReadsAllLinesFromStreamAsync()
    {
        const string content = """
            Line 1
            Line 2
            Line 3
            """;

        string[] expected = ["Line 1", "Line 2", "Line 3"];
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var actual = await TextFileUtility.ReadLinesAsync(stream, CancellationToken.None);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task ReadLinesAsync_EmptyStream_ReturnsEmptyListAsync()
    {
        using var stream = new MemoryStream();
        var actual = await TextFileUtility.ReadLinesAsync(stream, CancellationToken.None);

        Assert.That(actual, Is.Empty);
    }

    [Test]
    public async Task ReadLinesAsync_PreservesWhitespaceAsync()
    {
        const string content = """
              Line 1
                    Line 2
             Line 3    
            """;

        string[] expected = ["  Line 1", "        Line 2", " Line 3    "];
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var actual = await TextFileUtility.ReadLinesAsync(stream, CancellationToken.None);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task ReadLinesAsync_CancellationRequested_ThrowsAsync()
    {
        const string content = "Line 1";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var cancellationToken = cts.Token;

        await Assert.ThatAsync(
            () => TextFileUtility.ReadLinesAsync(stream, cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
        );
    }

    [Test]
    public async Task ReadLinesAsync_StreamAtNonZeroPosition_ReadsRemainingLinesAsync()
    {
        const string content = "Line 1\nLine 2\nLine 3\n";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        stream.Position = 7; // Position after "Line 1\n"

        var actual = await TextFileUtility.ReadLinesAsync(stream, CancellationToken.None);

        Assert.That(actual, Is.EqualTo(["Line 2", "Line 3"]));
    }

    [Test]
    public async Task WriteLinesAsync_WritesAllLinesToStreamAsync()
    {
        string[] lines = ["Line 1", "Line 2", "Line 3"];
        using var stream = new MemoryStream();

        await TextFileUtility.WriteLinesAsync(stream, lines, CancellationToken.None);
        stream.Position = 0;

        using var streamReader = new StreamReader(stream);
        var actual = await streamReader.ReadToEndAsync();
        var expected = string.Join(Environment.NewLine, lines) + Environment.NewLine;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task WriteLinesAsync_EmptyList_WritesNothingAsync()
    {
        using var stream = new MemoryStream();

        await TextFileUtility.WriteLinesAsync(stream, [], CancellationToken.None);
        stream.Position = 0;

        using var streamReader = new StreamReader(stream);
        var actual = await streamReader.ReadToEndAsync();

        Assert.That(actual, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task WriteLinesAsync_PreservesWhitespaceAsync()
    {
        string[] lines = ["  Line 1  ", "        Line 2", "Line 3    "];
        using var stream = new MemoryStream();

        await TextFileUtility.WriteLinesAsync(stream, lines, CancellationToken.None);
        stream.Position = 0;

        using var streamReader = new StreamReader(stream);
        var actual = await streamReader.ReadToEndAsync();
        var expected = string.Join(Environment.NewLine, lines) + Environment.NewLine;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task WriteLinesAsync_CancellationRequested_ThrowsAsync()
    {
        string[] lines = ["Line 1", "Line 2"];
        using var stream = new MemoryStream();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var cancellationToken = cts.Token;

        await Assert.ThatAsync(
            () => TextFileUtility.WriteLinesAsync(stream, lines, cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
        );
    }
}
