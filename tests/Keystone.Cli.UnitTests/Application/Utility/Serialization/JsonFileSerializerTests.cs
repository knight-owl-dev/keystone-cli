using System.Text;
using System.Text.Json;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility.Serialization;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Keystone.Cli.UnitTests.Application.Utility.Serialization;

[TestFixture, Parallelizable(ParallelScope.All)]
public class JsonFileSerializerTests
{
    private static JsonFileSerializer Ctor(IFileSystemService? fileSystemService = null)
        => new(fileSystemService ?? Substitute.For<IFileSystemService>());

    private sealed record TestModel
    {
        public int Value { get; init; }
    }

    [Test]
    public async Task LoadAsync_FileDoesNotExist_ThrowsFileNotFoundExceptionAsync()
    {
        const string path = "nonexistent.json";

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenReadStream(path).Throws<FileNotFoundException>();

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.LoadAsync<object>(path),
            Throws.TypeOf<FileNotFoundException>()
        );
    }

    [Test]
    public async Task LoadAsync_ValidJson_ReturnsDeserializedObjectAsync()
    {
        const string path = "test.json";

        var source = new TestModel
        {
            Value = 42,
        };

        var json = JsonSerializer.Serialize(source);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(path).Returns(true);
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);
        var actual = await sut.LoadAsync<TestModel>(path);

        Assert.That(actual, Is.EqualTo(source));
    }

    [Test]
    public async Task LoadAsync_InvalidJson_ThrowsInvalidOperationExceptionAsync()
    {
        const string path = "bad.json";

        var stream = new MemoryStream("not a json"u8.ToArray());
        var fileSystemService = Substitute.For<IFileSystemService>();

        fileSystemService.FileExists(path).Returns(true);
        fileSystemService.OpenReadStream(path).Returns(stream);

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.LoadAsync<TestModel>(path),
            Throws.TypeOf<InvalidOperationException>()
        );
    }

    [Test]
    public async Task SaveAsync_WritesJsonToFileAsync()
    {
        const string path = "save.json";

        var source = new TestModel
        {
            Value = 99,
        };

        await using var capturingStream = new CapturingStream();
        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(capturingStream);

        var sut = Ctor(fileSystemService);
        await sut.SaveAsync(path, source);

        using (Assert.EnterMultipleScope())
        {
            var actual = JsonSerializer.Deserialize<TestModel>(capturingStream.GetCapturedString());
            Assert.That(actual, Is.EqualTo(source));
        }
    }

    [Test]
    public async Task LoadAsync_CancellationRequested_ThrowsTaskCanceledExceptionAsync()
    {
        const string path = "test.json";

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        await cts.CancelAsync();

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(path).Returns(true);
        fileSystemService.OpenReadStream(path).Returns(new MemoryStream("{}"u8.ToArray()));

        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.LoadAsync<TestModel>(path, cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
        );
    }

    [Test]
    public async Task SaveAsync_CancellationRequested_ThrowsTaskCanceledExceptionAsync()
    {
        const string path = "save.json";

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        await cts.CancelAsync();

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.OpenWriteStream(path).Returns(new MemoryStream());
        var sut = Ctor(fileSystemService);

        await Assert.ThatAsync(
            () => sut.SaveAsync(path, new TestModel(), cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
        );
    }
}
