using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.UnitTests.Domain.Policies;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectModelLoadPolicyTests
{
    [Test]
    public void IsLoaded_WhenProjectModelIsNull_ReturnsFalse()
    {
        var actual = ProjectModelLoadPolicy.IsLoaded(projectModel: null);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsLoaded_WhenProjectNameIsNull_ReturnsFalse()
    {
        var model = new ProjectModel("/tmp")
        {
            ProjectName = null,
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        var actual = ProjectModelLoadPolicy.IsLoaded(model);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsLoaded_WhenProjectNameIsEmpty_ReturnsFalse()
    {
        var model = new ProjectModel("/tmp")
        {
            ProjectName = string.Empty,
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        var actual = ProjectModelLoadPolicy.IsLoaded(model);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsLoaded_WhenKeystoneSyncIsNull_ReturnsFalse()
    {
        var model = new ProjectModel("/tmp")
        {
            ProjectName = "my-project",
            KeystoneSync = null,
        };

        var actual = ProjectModelLoadPolicy.IsLoaded(model);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void IsLoaded_WhenProjectNameAndKeystoneSyncAreValid_ReturnsTrue()
    {
        var model = new ProjectModel("/tmp")
        {
            ProjectName = "my-project",
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        var actual = ProjectModelLoadPolicy.IsLoaded(model);

        Assert.That(actual, Is.True);
    }
}
