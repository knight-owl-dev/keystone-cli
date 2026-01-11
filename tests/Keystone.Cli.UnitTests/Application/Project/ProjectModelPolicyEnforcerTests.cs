using Keystone.Cli.Application.Project;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.UnitTests.Application.Project;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectModelPolicyEnforcerTests
{
    private readonly ProjectModelPolicyEnforcer _sut = new();

    [Test]
    public void ThrowIfNotLoaded_WithValidProjectModel_DoesNotThrow()
    {
        var validModel = new ProjectModel("/tmp")
        {
            ProjectName = "my-project",
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        Assume.That(ProjectModelLoadPolicy.IsLoaded(validModel), Is.True);

        Assert.That(
            () => _sut.ThrowIfNotLoaded(validModel),
            Throws.Nothing
        );
    }

    [Test]
    public void ThrowIfNotLoaded_WithInvalidProjectModel_ThrowsProjectNotLoadedException()
    {
        var invalidModel = new ProjectModel("/tmp");
        Assume.That(ProjectModelLoadPolicy.IsLoaded(invalidModel), Is.False);

        Assert.That(
            () => _sut.ThrowIfNotLoaded(invalidModel),
            Throws.TypeOf<ProjectNotLoadedException>().With.Message.Contains("/tmp")
        );
    }
}
