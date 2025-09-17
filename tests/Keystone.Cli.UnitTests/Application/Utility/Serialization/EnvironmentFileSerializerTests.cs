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

    // TODO: test SaveAsync method
}
