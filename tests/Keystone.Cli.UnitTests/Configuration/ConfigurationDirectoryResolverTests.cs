using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Application.Platform;
using Keystone.Cli.Configuration;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Configuration;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ConfigurationDirectoryResolverTests
{
    private const string EnvVarName = "KEYSTONE_CLI_CONFIG_DIR";
    private const string ConfigFileName = "appsettings.json";
    private const string UserConfigPath = "/home/user/.config/keystone-cli";
    private const string FhsPath = "/etc/keystone-cli";
    private const string AppBasePath = "/usr/local/bin";

    private sealed record Mocks(IEnvironmentService Environment, IFileSystemService FileSystem);

    private static Mocks CreateMocks(bool isWindows = false)
    {
        var environment = Substitute.For<IEnvironmentService>();
        var fileSystem = Substitute.For<IFileSystemService>();

        environment.IsWindows.Returns(isWindows);
        environment.AppBaseDirectory.Returns(AppBasePath);
        environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Returns("/home/user/.config");

        return new Mocks(environment, fileSystem);
    }

    private static ConfigurationDirectoryResolver Ctor(Mocks mocks)
        => new(mocks.Environment, mocks.FileSystem);

    [Test]
    public void ReturnsEnvVarPath_WhenSetAndContainsConfig()
    {
        const string customPath = "/custom/path";

        var mocks = CreateMocks();
        mocks.FileSystem.FileExists(Path.Combine(customPath, ConfigFileName)).Returns(true);
        mocks.Environment.GetEnvironmentVariable(EnvVarName).Returns(customPath);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(customPath));
    }

    [Test]
    public void IgnoresEnvVarPath_WhenSetButMissingConfig()
    {
        const string customPath = "/custom/path";

        var mocks = CreateMocks();
        mocks.FileSystem.FileExists(Path.Combine(customPath, ConfigFileName)).Returns(false);
        mocks.Environment.GetEnvironmentVariable(EnvVarName).Returns(customPath);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(AppBasePath));
    }

    [Test]
    public void IgnoresEnvVarPath_WhenEmpty()
    {
        var mocks = CreateMocks();
        mocks.Environment.GetEnvironmentVariable(EnvVarName).Returns(string.Empty);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(AppBasePath));
    }

    [Test]
    public void IgnoresEnvVarPath_WhenNull()
    {
        var mocks = CreateMocks();

        mocks.Environment.GetEnvironmentVariable(EnvVarName).Returns((string?) null);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(AppBasePath));
    }

    [Test]
    public void ReturnsUserConfigPath_WhenContainsConfig()
    {
        var mocks = CreateMocks();
        mocks.FileSystem.FileExists(Path.Combine(UserConfigPath, ConfigFileName)).Returns(true);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(UserConfigPath));
    }

    [Test]
    public void ReturnsUserConfigPath_OnWindows_WhenContainsConfig()
    {
        var mocks = CreateMocks(isWindows: true);
        mocks.FileSystem.FileExists(Path.Combine(UserConfigPath, ConfigFileName)).Returns(true);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(UserConfigPath));
    }

    [Test]
    public void ReturnsFhsPath_WhenUserConfigMissingAndFhsContainsConfig()
    {
        var mocks = CreateMocks();
        mocks.FileSystem.FileExists(Path.Combine(UserConfigPath, ConfigFileName)).Returns(false);
        mocks.FileSystem.FileExists(Path.Combine(FhsPath, ConfigFileName)).Returns(true);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(FhsPath));
    }

    [Test]
    public void SkipsFhsPath_OnWindows()
    {
        var mocks = CreateMocks(isWindows: true);
        mocks.FileSystem.FileExists(Path.Combine(UserConfigPath, ConfigFileName)).Returns(false);
        mocks.FileSystem.FileExists(Path.Combine(FhsPath, ConfigFileName)).Returns(true);

        var sut = Ctor(mocks);

        // Should return AppBasePath, not FhsPath, because FHS is skipped on Windows
        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(AppBasePath));
    }

    [Test]
    public void ReturnsAppBaseDirectory_AsFallback()
    {
        var mocks = CreateMocks();

        // No paths contain the config file

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(AppBasePath));
    }

    [Test]
    public void ReturnsAppBaseDirectory_AsFallback_OnWindows()
    {
        var mocks = CreateMocks(isWindows: true);

        // No paths contain the config file

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(AppBasePath));
    }

    [Test]
    public void EnvVarPath_TakesPrecedenceOverUserConfig()
    {
        const string customPath = "/custom/path";

        var mocks = CreateMocks();
        mocks.FileSystem.FileExists(Path.Combine(customPath, ConfigFileName)).Returns(true);
        mocks.FileSystem.FileExists(Path.Combine(UserConfigPath, ConfigFileName)).Returns(true);
        mocks.Environment.GetEnvironmentVariable(EnvVarName).Returns(customPath);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(customPath));
    }

    [Test]
    public void UserConfigPath_TakesPrecedenceOverFhs()
    {
        var mocks = CreateMocks();
        mocks.FileSystem.FileExists(Path.Combine(UserConfigPath, ConfigFileName)).Returns(true);
        mocks.FileSystem.FileExists(Path.Combine(FhsPath, ConfigFileName)).Returns(true);

        var sut = Ctor(mocks);

        Assert.That(sut.ResolveConfigDirectory(), Is.EqualTo(UserConfigPath));
    }

    [Test]
    public void DefaultInstance_IsNotNull()
        => Assert.That(ConfigurationDirectoryResolver.Default, Is.Not.Null);
}
