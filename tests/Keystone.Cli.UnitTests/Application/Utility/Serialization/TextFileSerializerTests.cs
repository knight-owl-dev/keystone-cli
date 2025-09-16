using System.Text;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility.Serialization;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Keystone.Cli.UnitTests.Application.Utility.Serialization;

[TestFixture, Parallelizable(ParallelScope.All)]
public class TextFileSerializerTests
{
    private static TextFileSerializer Ctor(IFileSystemService? fileSystemService = null)
        => new(fileSystemService ?? Substitute.For<IFileSystemService>());

    [Test]
    public async Task LoadLinesAsync_FileDoesNotExist_ThrowsFileNotFoundExceptionAsync()
    {
        const string path = "nonexistent.txt";

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(path).Returns(false);

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.LoadLinesAsync(path),
            Throws.TypeOf<FileNotFoundException>()
        );
    }

    [Test]
    public async Task LoadLinesAsync_ValidTextFile_ReturnsContentLinesAsync()
    {
        const string path = "test.txt";

        const string content = """
            # This is a comment
            Line 1

            # Another comment
            Line 2
            Line 3

            # Final comment
            """;

        string[] expectedLines = ["Line 1", "Line 2", "Line 3"];

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(path).Returns(true);
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        var actualLines = await sut.LoadLinesAsync(path);

        Assert.That(actualLines, Is.EqualTo(expectedLines));
    }

    [Test]
    public async Task LoadLinesAsync_EmptyFile_ReturnsEmptyListAsync()
    {
        const string path = "empty.txt";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(path).Returns(true);
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        var actualLines = await sut.LoadLinesAsync(path);

        Assert.That(actualLines, Is.Empty);
    }

    [Test]
    public async Task LoadLinesAsync_DoesNotTrimLinesAsync()
    {
        const string path = "whitespace.txt";

        const string content = """
            Line 1    
               Line 2
            Line 3   
            """;

        string[] expectedLines = ["Line 1    ", "   Line 2", "Line 3   "];

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(path).Returns(true);
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        var actualLines = await sut.LoadLinesAsync(path);

        Assert.That(actualLines, Is.EqualTo(expectedLines));
    }

    [Test]
    public async Task LoadLinesAsync_CancellationRequested_ThrowsOperationCanceledExceptionAsync()
    {
        const string path = "test.txt";

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        await cts.CancelAsync();

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(path).Returns(true);
        fileSystemService.OpenReadStream(path).Returns(new MemoryStream("line"u8.ToArray()));

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.LoadLinesAsync(path, cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
        );
    }

    [Test]
    public async Task SaveLinesAsync_BasicSave_WritesLinesCorrectlyAsync()
    {
        const string path = "output.txt";
        string[] lines = ["Line 1", "Line 2", "Line 3"];

        var capturingStream = new CapturingStream();
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(capturingStream);

        var sut = Ctor(fileSystemService);

        await sut.SaveLinesAsync(path, lines);

        var actual = Encoding.UTF8.GetString(capturingStream.CapturedBuffer ?? []);
        var expected = string.Join(Environment.NewLine, lines) + Environment.NewLine;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task SaveLinesAsync_EmptyList_WritesNoLinesAsync()
    {
        const string path = "empty_output.txt";

        var capturingStream = new CapturingStream();
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(capturingStream);

        var sut = Ctor(fileSystemService);

        await sut.SaveLinesAsync(path, lines: []);

        var actual = Encoding.UTF8.GetString(capturingStream.CapturedBuffer ?? []);

        Assert.That(actual, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task SaveLinesAsync_PreservesWhitespaceInLinesAsync()
    {
        const string path = "whitespace_output.txt";
        string[] lines = ["Line 1    ", "   Line 2", "Line 3   "];

        var capturingStream = new CapturingStream();
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(capturingStream);

        var sut = Ctor(fileSystemService);

        await sut.SaveLinesAsync(path, lines);

        var actual = Encoding.UTF8.GetString(capturingStream.CapturedBuffer ?? []);
        var expected = string.Join(Environment.NewLine, lines) + Environment.NewLine;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task SaveLinesAsync_OverwritesContent_KeepsBothHeaderAndFooterAsync()
    {
        const string path = "content_output.txt";

        const string content = """
            # This is a comment
            # -----------------

            Line 2
            Line 1
            Line 3

            # -----------------
            # Final comment
            """;

        const string expected = """
            # This is a comment
            # -----------------

            Line 1
            Line 2

            # -----------------
            # Final comment
            """;

        string[] lines = ["Line 1", "Line 2"];

        var capturingStream = new CapturingStream();
        capturingStream.Write(Encoding.UTF8.GetBytes(content));
        capturingStream.Position = 0;

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(capturingStream);

        var sut = Ctor(fileSystemService);

        await sut.SaveLinesAsync(path, lines);

        var actual = Encoding.UTF8.GetString(capturingStream.CapturedBuffer ?? []);

        Assert.That(actual, Is.EqualTo(expected).IgnoreWhiteSpace);
    }

    [Test]
    public async Task SaveLinesAsync_OverwritesContent_HeaderOnlyAsync()
    {
        const string path = "header_only_output.txt";

        const string content = """
            # This is a comment
            # -----------------
            """;

        const string expected = """
            # This is a comment
            # -----------------
            Line 1
            Line 2
            """;

        string[] lines = ["Line 1", "Line 2"];

        var capturingStream = new CapturingStream();
        capturingStream.Write(Encoding.UTF8.GetBytes(content));
        capturingStream.Position = 0;

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(capturingStream);

        var sut = Ctor(fileSystemService);

        await sut.SaveLinesAsync(path, lines);

        var actual = Encoding.UTF8.GetString(capturingStream.CapturedBuffer ?? []);

        Assert.That(actual, Is.EqualTo(expected).IgnoreWhiteSpace);
    }

    [Test]
    public async Task SaveLinesAsync_OverwritesContent_KeepsFooterAsync()
    {
        const string path = "footer_only_output.txt";

        const string content = """
            Line 2
            Line 1
            Line 3
            # -----------------
            # Final comment
            """;

        const string expected = """
            Line 1
            Line 2
            # -----------------
            # Final comment
            """;

        string[] lines = ["Line 1", "Line 2"];

        var capturingStream = new CapturingStream();
        capturingStream.Write(Encoding.UTF8.GetBytes(content));
        capturingStream.Position = 0;

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(capturingStream);

        var sut = Ctor(fileSystemService);

        await sut.SaveLinesAsync(path, lines);

        var actual = Encoding.UTF8.GetString(capturingStream.CapturedBuffer ?? []);

        Assert.That(actual, Is.EqualTo(expected).IgnoreWhiteSpace);
    }

    [Test]
    public async Task SaveLinesAsync_CancellationRequested_ThrowsOperationCanceledExceptionAsync()
    {
        const string path = "output.txt";
        string[] lines = ["Line 1", "Line 2"];

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        await cts.CancelAsync();

        var memoryStream = new MemoryStream();
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(memoryStream);

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.SaveLinesAsync(path, lines, cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
        );
    }

    [Test]
    public async Task SaveLinesAsync_DirectoryDoesNotExist_ThrowsDirectoryNotFoundExceptionAsync()
    {
        const string path = "missing_dir/output.txt";

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Throws<DirectoryNotFoundException>();

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.SaveLinesAsync(path, lines: ["Line 1"]),
            Throws.TypeOf<DirectoryNotFoundException>()
        );
    }

    [Test]
    public async Task SaveLinesAsync_UnauthorizedAccess_ThrowsUnauthorizedAccessExceptionAsync()
    {
        const string path = "unauthorized.txt";

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Throws<UnauthorizedAccessException>();

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.SaveLinesAsync(path, lines: ["Line 1"]),
            Throws.TypeOf<UnauthorizedAccessException>()
        );
    }
}
