using System.Text;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility.Serialization;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Keystone.Cli.UnitTests.Application.Utility.Serialization;

[TestFixture, Parallelizable(ParallelScope.All)]
public class EnvironmentFileSerializerTests
{
    private static EnvironmentFileSerializer Ctor(IFileSystemService? fileSystemService = null)
        => new(fileSystemService ?? Substitute.For<IFileSystemService>());

    [Test]
    public async Task LoadAsync_FileDoesNotExist_ThrowsFileNotFoundExceptionAsync()
    {
        const string path = "nonexistent.env";

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenReadStream(path).Throws<FileNotFoundException>();

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.LoadAsync(path),
            Throws.TypeOf<FileNotFoundException>()
        );
    }

    [Test]
    public async Task LoadAsync_ValidEnvFile_ReturnsKeyValuePairsAsync()
    {
        const string path = "test.env";

        const string content = """
            # Comment
            KEY1=VALUE1

            # Another comment
            KEY2=VALUE2
            KEY3=VALUE3

            # Final comment
            """;

        var expected = new Dictionary<string, string?>
        {
            ["KEY1"] = "VALUE1",
            ["KEY2"] = "VALUE2",
            ["KEY3"] = "VALUE3",
        };

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        var actual = await sut.LoadAsync(path);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task LoadAsync_SameKeyMultipleTimes_ThrowsInvalidOperationExceptionAsync()
    {
        const string path = "duplicate.env";

        const string content = """
            KEY1=VALUE1
            KEY2=VALUE2
            KEY1=VALUE3
            """;

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.LoadAsync(path),
            Throws.InstanceOf<InvalidOperationException>().And.Message.Contains("KEY1")
        );
    }

    [Test]
    public async Task LoadAsync_EmptyFile_ReturnsEmptyDictionaryAsync()
    {
        const string path = "empty.env";

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        var actual = await sut.LoadAsync(path);

        Assert.That(actual, Is.Empty);
    }

    [Test]
    public async Task LoadAsync_TrimsKeysAndValuesAsync()
    {
        const string path = "whitespace.env";
        const string content = """
            KEY1=VALUE1   
               KEY2 = VALUE2
            KEY3=   VALUE3   
            """;

        var expected = new Dictionary<string, string?>
        {
            ["KEY1"] = "VALUE1",
            ["KEY2"] = "VALUE2",
            ["KEY3"] = "VALUE3",
        };

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        var actual = await sut.LoadAsync(path);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task LoadAsync_CancellationRequested_ThrowsOperationCanceledExceptionAsync()
    {
        const string path = "test.env";

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        await cts.CancelAsync();

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenReadStream(path).Returns(new MemoryStream("KEY=VALUE"u8.ToArray()));

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.LoadAsync(path, cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
        );
    }

    [Test]
    public async Task SaveAsync_UpdatesAndAddsKeys_PreservesCommentsAndBlankLinesAsync()
    {
        const string path = "test.env";

        const string content = """
            # Comment
            KEY1=OLD-VALUE1

            # Another comment
            KEY2=OLD-VALUE2
            KEY3 = OLD-VALUE3
            KEY5=NOT-TO-BE-CHANGED

            # Final comment
            """;

        var values = new Dictionary<string, string?>
        {
            ["KEY1"] = "NEW-VALUE1", // update
            ["KEY2"] = "NEW-VALUE2", // update
            ["KEY3"] = "OLD-VALUE3", // no change
            ["KEY4"] = "NEW-VALUE4", // add
        };

        const string expected = """
            # Comment
            KEY1=NEW-VALUE1

            # Another comment
            KEY2=NEW-VALUE2
            KEY3 = OLD-VALUE3
            KEY5=NOT-TO-BE-CHANGED

            # Final comment
            KEY4=NEW-VALUE4
            """;

        var stream = new CapturingStream().SetContent(content);
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        await sut.SaveAsync(path, values);
        var actual = stream.GetCapturedString();

        Assert.That(actual, Is.EqualTo(expected).IgnoreWhiteSpace);
    }
}
