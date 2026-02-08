using System.Diagnostics.CodeAnalysis;
using Keystone.Cli.Application.Data.Stores;
using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Application.Utility.Serialization;
using Keystone.Cli.Domain.Project;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Data.Stores;

[TestFixture, Parallelizable(ParallelScope.All)]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class ProjectConfFileProjectModelStoreTests
{
    private static ProjectConfFileProjectModelStore Ctor(
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
    public async Task LoadAsync_ProjectConfFileDoesNotExist_ReturnsModelUnchangedAsync()
    {
        const string projectPath = "/test/project";

        var model = new ProjectModel(projectPath)
        {
            ProjectName = "existing-project",
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(Arg.Is<string>(path => path.EndsWith("project.conf"))).Returns(false);

        var sut = Ctor(fileSystemService: fileSystemService);

        var actual = await sut.LoadAsync(model);

        Assert.That(actual.ProjectName, Is.EqualTo("existing-project"));
    }

    [Test]
    public async Task LoadAsync_ProjectConfFileExists_ReturnsModelWithEnvironmentValuesAsync()
    {
        const string projectPath = "/test/project";
        var projectConfFilePath = Path.Combine(projectPath, "project.conf");
        var model = new ProjectModel(projectPath);

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var envValues = new Dictionary<string, string?>
        {
            ["KEYSTONE_PROJECT"] = "test-project",
            ["KEYSTONE_DOCKER_COMPOSE_PROJECT"] = "keystone-test-project",
            ["KEYSTONE_DOCKER_IMAGE"] = "keystone-test-project",
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(projectConfFilePath).Returns(true);

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        environmentFileSerializer.LoadAsync(projectConfFilePath, cancellationToken).Returns(envValues);

        var sut = Ctor(fileSystemService: fileSystemService, environmentFileSerializer: environmentFileSerializer);

        var actual = await sut.LoadAsync(model, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ProjectName, Is.EqualTo("test-project"));
            Assert.That(actual.DockerComposeProject, Is.EqualTo("keystone-test-project"));
            Assert.That(actual.DockerImage, Is.EqualTo("keystone-test-project"));
        }
    }

    [Test]
    public async Task LoadAsync_EmptyProjectConfFile_ResetsPropertiesAsync()
    {
        const string projectPath = "/test/project";
        var projectConfFilePath = Path.Combine(projectPath, "project.conf");

        var model = new ProjectModel(projectPath)
        {
            ProjectName = "existing-project",
            DockerComposeProject = "keystone-existing-project",
            DockerImage = "keystone-existing-project",
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(projectConfFilePath).Returns(true);

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        environmentFileSerializer.LoadAsync(projectConfFilePath).Returns(new Dictionary<string, string?>());

        var sut = Ctor(fileSystemService: fileSystemService, environmentFileSerializer: environmentFileSerializer);

        var actual = await sut.LoadAsync(model);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ProjectName, Is.Null);
            Assert.That(actual.DockerComposeProject, Is.Null);
            Assert.That(actual.DockerImage, Is.Null);
        }
    }

    [Test]
    public async Task LoadAsync_EmptyAndWhiteSpaceStringValues_PreservesAllValuesAsync()
    {
        const string projectPath = "/test/project";
        var projectConfFilePath = Path.Combine(projectPath, "project.conf");
        var model = new ProjectModel(projectPath);

        var envValues = new Dictionary<string, string?>
        {
            ["KEYSTONE_PROJECT"] = "",
            ["KEYSTONE_DOCKER_COMPOSE_PROJECT"] = null,
            ["KEYSTONE_DOCKER_IMAGE"] = "   ",
        };

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.FileExists(projectConfFilePath).Returns(true);

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        environmentFileSerializer.LoadAsync(projectConfFilePath).Returns(envValues);

        var sut = Ctor(fileSystemService: fileSystemService, environmentFileSerializer: environmentFileSerializer);

        var actual = await sut.LoadAsync(model);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ProjectName, Is.Empty, "Empty string");
            Assert.That(actual.DockerComposeProject, Is.Null, "Null string");
            Assert.That(actual.DockerImage, Is.EqualTo("   "), "White-space only");
        }
    }

    [Test]
    public async Task SaveAsync_AllProperties_SavesAllEnvironmentValuesAsync()
    {
        const string projectPath = "/test/project";
        var projectConfFilePath = Path.Combine(projectPath, "project.conf");

        var model = new ProjectModel(projectPath)
        {
            ProjectName = "test-project",
            DockerComposeProject = "keystone-test-project",
            DockerImage = "keystone-test-project",
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        var sut = Ctor(environmentFileSerializer: environmentFileSerializer);

        await sut.SaveAsync(model, cancellationToken);

        await environmentFileSerializer.Received(1).SaveAsync(
            projectConfFilePath,
            Arg.Is<IDictionary<string, string?>>(dict =>
                dict["KEYSTONE_PROJECT"] == "test-project"
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
        var projectConfFilePath = Path.Combine(projectPath, "project.conf");

        var model = new ProjectModel(projectPath)
        {
            ProjectName = null,
            DockerComposeProject = null,
            DockerImage = null,
        };

        var environmentFileSerializer = Substitute.For<IEnvironmentFileSerializer>();
        var sut = Ctor(environmentFileSerializer: environmentFileSerializer);

        await sut.SaveAsync(model);

        await environmentFileSerializer.Received(1).SaveAsync(
            projectConfFilePath,
            Arg.Is<IDictionary<string, string?>>(dict => dict.All(kvp => kvp.Value == null)),
            Arg.Any<CancellationToken>()
        );
    }

    [Test]
    public void GetContentHash_AllProperties_ReturnsHash()
    {
        const string projectPath = "/test/project";
        const string expectedHash = "test-hash";

        var model = new ProjectModel(projectPath)
        {
            ProjectName = "test-project",
            DockerComposeProject = "keystone-test-project",
            DockerImage = "keystone-test-project",
        };

        var contentHashService = Substitute.For<IContentHashService>();
        contentHashService.ComputeFromKeyValues(
            Arg.Is<IDictionary<string, string?>>(dict =>
                dict["KEYSTONE_PROJECT"] == "test-project"
                && dict["KEYSTONE_DOCKER_COMPOSE_PROJECT"] == "keystone-test-project"
                && dict["KEYSTONE_DOCKER_IMAGE"] == "keystone-test-project"
            )
        ).Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }

    [Test]
    public void GetContentHash_NullProperties_ReturnsHash()
    {
        const string projectPath = "/test/project";
        const string expectedHash = "null-hash";

        var model = new ProjectModel(projectPath)
        {
            ProjectName = null,
            DockerComposeProject = null,
            DockerImage = null,
        };

        var contentHashService = Substitute.For<IContentHashService>();
        contentHashService
            .ComputeFromKeyValues(Arg.Is<IDictionary<string, string?>>(dict => dict.All(kvp => kvp.Value == null)))
            .Returns(expectedHash);

        var sut = Ctor(contentHashService: contentHashService);

        var actual = sut.GetContentHash(model);

        Assert.That(actual, Is.EqualTo(expectedHash));
    }
}
