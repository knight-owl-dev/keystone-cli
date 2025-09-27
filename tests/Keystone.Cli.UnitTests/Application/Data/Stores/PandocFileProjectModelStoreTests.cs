using System.Diagnostics.CodeAnalysis;
using Keystone.Cli.Application.Data.Stores;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Application.Utility.Serialization.Yaml;
using Keystone.Cli.Domain.Project;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Data.Stores;

[TestFixture, Parallelizable(ParallelScope.All)]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class PandocFileProjectModelStoreTests
{
    private static PandocFileProjectModelStore Ctor(
        IContentHashService? contentHashService = null,
        IFileSystemService? fileSystemService = null,
        IYamlFileSerializer? yamlFileSerializer = null
    )
        => new(
            contentHashService ?? Substitute.For<IContentHashService>(),
            fileSystemService ?? Substitute.For<IFileSystemService>(),
            yamlFileSerializer ?? Substitute.For<IYamlFileSerializer>()
        );

    [Test]
    public async Task LoadAsync_PandocFileDoesNotExist_ReturnsModelUnchangedAsync()
    {
        const string projectPath = "/test/project";
        var expectedPandocFilePath = Path.Combine(projectPath, ProjectFiles.PandocFileName);

        var model = new ProjectModel(projectPath)
        {
            Title = "existing-title",
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(expectedPandocFilePath).Returns(false);

        var sut = Ctor(fileSystemService: fileSystemService);

        var actual = await sut.LoadAsync(model);

        Assert.That(actual.Title, Is.EqualTo("existing-title"));
    }

    [Test]
    public async Task LoadAsync_PandocFileExists_ReturnsModelWithPandocDataAsync()
    {
        const string projectPath = "/test/project";
        var expectedPandocFilePath = Path.Combine(projectPath, ProjectFiles.PandocFileName);

        var model = new ProjectModel(projectPath);

        var yamlData = new Dictionary<string, YamlValue>
        {
            ["title"] = new YamlScalar("Test Book"),
            ["subtitle"] = new YamlScalar("A Sample Book"),
            ["author"] = new YamlScalar("John Doe"),
            ["date"] = new YamlScalar("2025-01-01"),
            ["lang"] = new YamlScalar("en-US"),
            ["footer-copyright"] = new YamlScalar("auto"),
            ["description"] = new YamlScalar("A test book description"),
            ["keywords"] = new YamlArray(["test", "book", "example"]),
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(expectedPandocFilePath).Returns(true);

        var yamlFileSerializer = Substitute.For<IYamlFileSerializer>();
        yamlFileSerializer
            .LoadAsync(expectedPandocFilePath, cancellationToken)
            .Returns(yamlData);

        var sut = Ctor(fileSystemService: fileSystemService, yamlFileSerializer: yamlFileSerializer);

        var actual = await sut.LoadAsync(model, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Title, Is.EqualTo("Test Book"));
            Assert.That(actual.Subtitle, Is.EqualTo("A Sample Book"));
            Assert.That(actual.Author, Is.EqualTo("John Doe"));
            Assert.That(actual.Date, Is.EqualTo("2025-01-01"));
            Assert.That(actual.Lang, Is.EqualTo("en-US"));
            Assert.That(actual.FooterCopyright, Is.EqualTo("auto"));
            Assert.That(actual.Description, Is.EqualTo("A test book description"));
            Assert.That(actual.Keywords, Is.EqualTo(["test", "book", "example"]));
        }
    }

    [Test]
    public async Task LoadAsync_PandocFileExistsWithPartialData_ReturnsModelWithAvailableDataAsync()
    {
        const string projectPath = "/test/project";
        var expectedPandocFilePath = Path.Combine(projectPath, ProjectFiles.PandocFileName);

        var model = new ProjectModel(projectPath);

        var yamlData = new Dictionary<string, YamlValue>
        {
            ["title"] = new YamlScalar("Partial Book"),
            ["author"] = new YamlScalar("Jane Doe"),
            ["keywords"] = new YamlArray(["partial"]),
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(expectedPandocFilePath).Returns(true);

        var yamlFileSerializer = Substitute.For<IYamlFileSerializer>();
        yamlFileSerializer
            .LoadAsync(expectedPandocFilePath, Arg.Any<CancellationToken>())
            .Returns(yamlData);

        var sut = Ctor(fileSystemService: fileSystemService, yamlFileSerializer: yamlFileSerializer);

        var actual = await sut.LoadAsync(model);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Title, Is.EqualTo("Partial Book"));
            Assert.That(actual.Author, Is.EqualTo("Jane Doe"));
            Assert.That(actual.Keywords, Is.EqualTo(["partial"]));
            Assert.That(actual.Subtitle, Is.Null);
            Assert.That(actual.Date, Is.Null);
            Assert.That(actual.Lang, Is.Null);
            Assert.That(actual.FooterCopyright, Is.Null);
            Assert.That(actual.Description, Is.Null);
        }
    }

    [Test]
    [SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
    public async Task SaveAsync_ModelHasPandocData_SavesPandocDataAsync()
    {
        const string projectPath = "/test/project";
        var expectedPandocFilePath = Path.Combine(projectPath, ProjectFiles.PandocFileName);

        var model = new ProjectModel(projectPath)
        {
            Title = "Save Test Book",
            Subtitle = "Testing Save",
            Author = "Test Author",
            Date = "auto",
            Lang = "en-GB",
            FooterCopyright = "disabled",
            Description = "Test description for saving",
            Keywords = ["save", "test"],
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var yamlFileSerializer = Substitute.For<IYamlFileSerializer>();

        var sut = Ctor(yamlFileSerializer: yamlFileSerializer);

        await sut.SaveAsync(model, cancellationToken);

        await yamlFileSerializer.Received(1).SaveAsync(
            expectedPandocFilePath,
            Arg.Is<IDictionary<string, YamlValue>>(dict =>
                dict["title"] == new YamlScalar("Save Test Book")
                && dict["subtitle"] == new YamlScalar("Testing Save")
                && dict["author"] == new YamlScalar("Test Author")
                && dict["date"] == new YamlScalar("auto")
                && dict["lang"] == new YamlScalar("en-GB")
                && dict["footer-copyright"] == new YamlScalar("disabled")
                && dict["description"] == new YamlScalar("Test description for saving")
                && dict["keywords"] == new YamlArray(new[] { "save", "test" })
            ),
            cancellationToken
        );
    }

    [Test]
    public async Task SaveAsync_ModelHasNullPandocData_SavesNullValuesAsync()
    {
        const string projectPath = "/test/project";
        var expectedPandocFilePath = Path.Combine(projectPath, ProjectFiles.PandocFileName);

        var model = new ProjectModel(projectPath);

        var yamlFileSerializer = Substitute.For<IYamlFileSerializer>();

        var sut = Ctor(yamlFileSerializer: yamlFileSerializer);

        await sut.SaveAsync(model);

        await yamlFileSerializer.Received(1).SaveAsync(
            expectedPandocFilePath,
            Arg.Is<IDictionary<string, YamlValue>>(dict =>
                dict["title"] == YamlScalar.Null
                && dict["subtitle"] == YamlScalar.Null
                && dict["author"] == YamlScalar.Null
                && dict["date"] == YamlScalar.Null
                && dict["lang"] == YamlScalar.Null
                && dict["footer-copyright"] == YamlScalar.Null
                && dict["description"] == YamlScalar.Null
                && dict["keywords"] == YamlScalar.Null
            ),
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public void GetContentHash_ModelHasNoPandocData_ReturnsHashOfNullValues()
    {
        const string projectPath = "/test/project";
        const string expectedHash = "null-hash";

        var model = new ProjectModel(projectPath);

        var contentHashService = Substitute.For<IContentHashService>();
        contentHashService.ComputeFromKeyValues(
            Arg.Is<Dictionary<string, string?>>(dict =>
                dict["title"] == null
                && dict["subtitle"] == null
                && dict["author"] == null
                && dict["date"] == null
                && dict["lang"] == null
                && dict["footer-copyright"] == null
                && dict["description"] == null
                && dict["keywords"] == null
            )
        ).Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }

    [Test]
    public void GetContentHash_ModelHasPandocData_ReturnsHashOfPandocValues()
    {
        const string projectPath = "/test/project";
        const string expectedHash = "pandoc-hash";

        var model = new ProjectModel(projectPath)
        {
            Title = "Hash Test Book",
            Subtitle = "Testing Hashing",
            Author = "Hash Author",
            Date = "2025-01-15",
            Lang = "fr-FR",
            FooterCopyright = "custom copyright",
            Description = "Hash test description",
            Keywords = ["hash", "test", "keywords"],
        };

        var contentHashService = Substitute.For<IContentHashService>();
        contentHashService.ComputeFromKeyValues(
            Arg.Is<Dictionary<string, string?>>(dict =>
                dict["title"] == "Hash Test Book"
                && dict["subtitle"] == "Testing Hashing"
                && dict["author"] == "Hash Author"
                && dict["date"] == "2025-01-15"
                && dict["lang"] == "fr-FR"
                && dict["footer-copyright"] == "custom copyright"
                && dict["description"] == "Hash test description"
                && dict["keywords"] == "hash,test,keywords"
            )
        ).Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }
}
