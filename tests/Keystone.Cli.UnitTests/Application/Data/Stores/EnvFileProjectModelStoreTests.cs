using Keystone.Cli.Application.Data.Stores;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Domain.Project;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Data.Stores;

[TestFixture, Parallelizable(ParallelScope.All)]
public class EnvFileProjectModelStoreTests
{
    private static EnvFileProjectModelStore Ctor(
        IContentHashService? contentHashService = null,
        IFileSystemService? fileSystemService = null,
        IEnvironmentFileSerializer? environmentFileSerializer = null
    )
        => new(
            contentHashService ?? Substitute.For<IContentHashService>(),
            fileSystemService ?? Substitute.For<IFileSystemService>(),
            environmentFileSerializer ?? Substitute.For<IEnvironmentFileSerializer>()
        );

    [Test]
    public async Task LoadAsync_EnvFileDoesNotExist_ReturnsModelUnchangedAsync()
    {
        const string projectPath = "/test/project";
        var model = new ProjectModel(projectPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(Arg.Is<string>(path => path.EndsWith(".env"))).Returns(false);

        var sut = Ctor(fileSystemService: fileSystemService);

        var actual = await sut.LoadAsync(model);

        Assert.That(actual, Is.SameAs(model));
    }

    [Test]
    public async Task LoadAsync_EnvFileExists_ReturnsModelWithEnvironmentValuesAsync()
    {
        const string projectPath = "/test/project";
        var envFilePath = Path.Combine(projectPath, ".env");
        var model = new ProjectModel(projectPath);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var envValues = new Dictionary<string, string?>
        {
            ["KEYSTONE_PROJECT"] = "test-project",
            ["KEYSTONE_COVER_IMAGE"] = "./assets/cover.png",
            ["KEYSTONE_LATEX_PAPERSIZE"] = "a4",
            ["KEYSTONE_LATEX_GEOMETRY"] = "margin=1in",
            ["KEYSTONE_LATEX_FONTSIZE"] = "12pt",
            ["KEYSTONE_LATEX_FONTFAMILY"] = "libertine",
            ["KEYSTONE_DOCKER_COMPOSE_PROJECT"] = "keystone-test-project",
            ["KEYSTONE_DOCKER_IMAGE"] = "keystone-test-project",
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(envFilePath).Returns(true);

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        environmentFileSerializer.LoadAsync(envFilePath, cancellationToken).Returns(envValues);

        var sut = Ctor(fileSystemService: fileSystemService, environmentFileSerializer: environmentFileSerializer);

        var actual = await sut.LoadAsync(model, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ProjectName, Is.EqualTo("test-project"));
            Assert.That(actual.CoverImage, Is.EqualTo("./assets/cover.png"));
            Assert.That(actual.LatexPapersize, Is.EqualTo("a4"));
            Assert.That(actual.LatexGeometry, Is.EqualTo("margin=1in"));
            Assert.That(actual.LatexFontsize, Is.EqualTo("12pt"));
            Assert.That(actual.LatexFontfamily, Is.EqualTo("libertine"));
            Assert.That(actual.DockerComposeProject, Is.EqualTo("keystone-test-project"));
            Assert.That(actual.DockerImage, Is.EqualTo("keystone-test-project"));
        }
    }

    [Test]
    public async Task LoadAsync_EmptyEnvFile_ReturnsModelUnchangedAsync()
    {
        const string projectPath = "/test/project";
        var envFilePath = Path.Combine(projectPath, ".env");
        var model = new ProjectModel(projectPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(envFilePath).Returns(true);

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        environmentFileSerializer.LoadAsync(envFilePath).Returns(new Dictionary<string, string?>());

        var sut = Ctor(fileSystemService: fileSystemService, environmentFileSerializer: environmentFileSerializer);

        var actual = await sut.LoadAsync(model);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ProjectName, Is.Null);
            Assert.That(actual.CoverImage, Is.Null);
            Assert.That(actual.LatexPapersize, Is.Null);
            Assert.That(actual.LatexGeometry, Is.Null);
            Assert.That(actual.LatexFontsize, Is.Null);
            Assert.That(actual.LatexFontfamily, Is.Null);
            Assert.That(actual.DockerComposeProject, Is.Null);
            Assert.That(actual.DockerImage, Is.Null);
        }
    }

    [Test]
    public async Task LoadAsync_PartialEnvFile_ReturnsModelWithPartialValuesAsync()
    {
        const string projectPath = "/test/project";
        var envFilePath = Path.Combine(projectPath, ".env");
        var model = new ProjectModel(projectPath);

        var envValues = new Dictionary<string, string?>
        {
            ["KEYSTONE_PROJECT"] = "test-project",
            ["KEYSTONE_LATEX_FONTSIZE"] = "12pt",
            ["KEYSTONE_DOCKER_IMAGE"] = "custom-image",
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(envFilePath).Returns(true);

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        environmentFileSerializer.LoadAsync(envFilePath).Returns(envValues);

        var sut = Ctor(fileSystemService: fileSystemService, environmentFileSerializer: environmentFileSerializer);

        var actual = await sut.LoadAsync(model);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ProjectName, Is.EqualTo("test-project"));
            Assert.That(actual.CoverImage, Is.Null);
            Assert.That(actual.LatexPapersize, Is.Null);
            Assert.That(actual.LatexGeometry, Is.Null);
            Assert.That(actual.LatexFontsize, Is.EqualTo("12pt"));
            Assert.That(actual.LatexFontfamily, Is.Null);
            Assert.That(actual.DockerComposeProject, Is.Null);
            Assert.That(actual.DockerImage, Is.EqualTo("custom-image"));
        }
    }

    [Test]
    public async Task LoadAsync_EmptyStringValues_TreatsAsNullAsync()
    {
        const string projectPath = "/test/project";
        var envFilePath = Path.Combine(projectPath, ".env");
        var model = new ProjectModel(projectPath);

        var envValues = new Dictionary<string, string?>
        {
            ["KEYSTONE_PROJECT"] = "",
            ["KEYSTONE_COVER_IMAGE"] = null,
            ["KEYSTONE_LATEX_PAPERSIZE"] = "   ",
            ["KEYSTONE_LATEX_GEOMETRY"] = "margin=1in",
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(envFilePath).Returns(true);

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        environmentFileSerializer.LoadAsync(envFilePath).Returns(envValues);

        var sut = Ctor(fileSystemService: fileSystemService, environmentFileSerializer: environmentFileSerializer);

        var actual = await sut.LoadAsync(model);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ProjectName, Is.Null);
            Assert.That(actual.CoverImage, Is.Null);
            Assert.That(actual.LatexPapersize, Is.EqualTo("   ")); // Whitespace-only is preserved
            Assert.That(actual.LatexGeometry, Is.EqualTo("margin=1in"));
        }
    }

    [Test]
    public async Task LoadAsync_OverwritesWithEnvValuesAsync()
    {
        const string projectPath = "/test/project";
        var envFilePath = Path.Combine(projectPath, ".env");

        var model = new ProjectModel(projectPath)
        {
            ProjectName = "existing-project",
            CoverImage = "./assets/existing-cover.png",
            LatexFontsize = "10pt",
        };

        var envValues = new Dictionary<string, string?>
        {
            ["KEYSTONE_PROJECT"] = "new-project",
            ["KEYSTONE_LATEX_FONTSIZE"] = "12pt",

            // No KEYSTONE_COVER_IMAGE, should not preserve existing value
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(envFilePath).Returns(true);

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        environmentFileSerializer.LoadAsync(envFilePath).Returns(envValues);

        var sut = Ctor(fileSystemService: fileSystemService, environmentFileSerializer: environmentFileSerializer);

        var actual = await sut.LoadAsync(model);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ProjectName, Is.EqualTo("new-project"));
            Assert.That(actual.CoverImage, Is.Null); // Not preserved
            Assert.That(actual.LatexFontsize, Is.EqualTo("12pt"));
        }
    }

    [Test]
    public async Task SaveAsync_AllProperties_SavesAllEnvironmentValuesAsync()
    {
        const string projectPath = "/test/project";
        var envFilePath = Path.Combine(projectPath, ".env");

        var model = new ProjectModel(projectPath)
        {
            ProjectName = "test-project",
            CoverImage = "./assets/cover.png",
            LatexPapersize = "a4",
            LatexGeometry = "margin=1in",
            LatexFontsize = "12pt",
            LatexFontfamily = "libertine",
            DockerComposeProject = "keystone-test-project",
            DockerImage = "keystone-test-project",
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        var sut = Ctor(environmentFileSerializer: environmentFileSerializer);

        await sut.SaveAsync(model, cancellationToken);

        await environmentFileSerializer.Received(1).SaveAsync(
            envFilePath,
            Arg.Is<IDictionary<string, string?>>(dict =>
                dict["KEYSTONE_PROJECT"] == "test-project"
                && dict["KEYSTONE_COVER_IMAGE"] == "./assets/cover.png"
                && dict["KEYSTONE_LATEX_PAPERSIZE"] == "a4"
                && dict["KEYSTONE_LATEX_GEOMETRY"] == "margin=1in"
                && dict["KEYSTONE_LATEX_FONTSIZE"] == "12pt"
                && dict["KEYSTONE_LATEX_FONTFAMILY"] == "libertine"
                && dict["KEYSTONE_DOCKER_COMPOSE_PROJECT"] == "keystone-test-project"
                && dict["KEYSTONE_DOCKER_IMAGE"] == "keystone-test-project"
            ),
            cancellationToken
        );
    }

    [Test]
    public async Task SaveAsync_NullProperties_SavesNullValuesAsync()
    {
        const string projectPath = "/test/project";
        var envFilePath = Path.Combine(projectPath, ".env");

        var model = new ProjectModel(projectPath)
        {
            ProjectName = null,
            CoverImage = null,
            LatexPapersize = null,
            LatexGeometry = null,
            LatexFontsize = null,
            LatexFontfamily = null,
            DockerComposeProject = null,
            DockerImage = null,
        };

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        var sut = Ctor(environmentFileSerializer: environmentFileSerializer);

        await sut.SaveAsync(model);

        await environmentFileSerializer.Received(1).SaveAsync(
            envFilePath,
            Arg.Is<IDictionary<string, string?>>(dict =>
                dict["KEYSTONE_PROJECT"] == null
                && dict["KEYSTONE_COVER_IMAGE"] == null
                && dict["KEYSTONE_LATEX_PAPERSIZE"] == null
                && dict["KEYSTONE_LATEX_GEOMETRY"] == null
                && dict["KEYSTONE_LATEX_FONTSIZE"] == null
                && dict["KEYSTONE_LATEX_FONTFAMILY"] == null
                && dict["KEYSTONE_DOCKER_COMPOSE_PROJECT"] == null
                && dict["KEYSTONE_DOCKER_IMAGE"] == null
            ),
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public void GetContentHash_AllProperties_ReturnsConsistentHash()
    {
        const string projectPath = "/test/project";
        const string expectedHash = "test-hash";

        var model = new ProjectModel(projectPath)
        {
            ProjectName = "test-project",
            CoverImage = "./assets/cover.png",
            LatexPapersize = "a4",
            LatexGeometry = "margin=1in",
            LatexFontsize = "12pt",
            LatexFontfamily = "libertine",
            DockerComposeProject = "keystone-test-project",
            DockerImage = "keystone-test-project",
        };

        var contentHashService = Substitute.For<IContentHashService>();
        contentHashService.ComputeFromKeyValues(
            Arg.Is<IDictionary<string, string?>>(dict =>
                dict["KEYSTONE_PROJECT"] == "test-project"
                && dict["KEYSTONE_COVER_IMAGE"] == "./assets/cover.png"
                && dict["KEYSTONE_LATEX_PAPERSIZE"] == "a4"
                && dict["KEYSTONE_LATEX_GEOMETRY"] == "margin=1in"
                && dict["KEYSTONE_LATEX_FONTSIZE"] == "12pt"
                && dict["KEYSTONE_LATEX_FONTFAMILY"] == "libertine"
                && dict["KEYSTONE_DOCKER_COMPOSE_PROJECT"] == "keystone-test-project"
                && dict["KEYSTONE_DOCKER_IMAGE"] == "keystone-test-project"
            )
        ).Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }

    [Test]
    public void GetContentHash_NullProperties_ReturnsHashOfNullValues()
    {
        const string projectPath = "/test/project";
        const string expectedHash = "null-hash";

        var model = new ProjectModel(projectPath)
        {
            ProjectName = null,
            CoverImage = null,
            LatexPapersize = null,
            LatexGeometry = null,
            LatexFontsize = null,
            LatexFontfamily = null,
            DockerComposeProject = null,
            DockerImage = null,
        };

        var contentHashService = Substitute.For<IContentHashService>();
        contentHashService.ComputeFromKeyValues(
            Arg.Is<IDictionary<string, string?>>(dict =>
                dict["KEYSTONE_PROJECT"] == null
                && dict["KEYSTONE_COVER_IMAGE"] == null
                && dict["KEYSTONE_LATEX_PAPERSIZE"] == null
                && dict["KEYSTONE_LATEX_GEOMETRY"] == null
                && dict["KEYSTONE_LATEX_FONTSIZE"] == null
                && dict["KEYSTONE_LATEX_FONTFAMILY"] == null
                && dict["KEYSTONE_DOCKER_COMPOSE_PROJECT"] == null
                && dict["KEYSTONE_DOCKER_IMAGE"] == null
            )
        ).Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }
}
