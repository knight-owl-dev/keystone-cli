using Keystone.Cli.Application.Data;
using Keystone.Cli.Application.Project;
using Keystone.Cli.Domain.Project;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Data;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectModelRepositoryTests
{
    private static ProjectModelRepository Ctor(
        IEnumerable<IProjectModelStore>? projectModelStores = null,
        IProjectModelPolicyEnforcer? projectModelPolicyEnforcer = null
    )
        => new(
            projectModelStores ?? [],
            projectModelPolicyEnforcer ?? Substitute.For<IProjectModelPolicyEnforcer>()
        );

    [Test]
    public async Task LoadAsync_ReturnsAggregatedModelAsync()
    {
        const string projectPath = "/path/to/project";

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var initialModel = new ProjectModel(projectPath);

        var modelFromStore1 = initialModel with
        {
            ProjectName = "my-project",
        };

        var modelFromStore2 = modelFromStore1 with
        {
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        var store1 = Substitute.For<IProjectModelStore>();
        store1.LoadAsync(initialModel, cancellationToken).Returns(modelFromStore1);

        var store2 = Substitute.For<IProjectModelStore>();
        store2.LoadAsync(modelFromStore1, cancellationToken).Returns(modelFromStore2);

        var sut = Ctor([store1, store2]);

        var actual = await sut.LoadAsync(projectPath, cancellationToken);

        Assert.That(actual, Is.EqualTo(modelFromStore2));
    }

    [Test]
    public async Task LoadAsync_EnforcesPolicyAsync()
    {
        const string projectPath = "/path/to/project";

        var initialModel = new ProjectModel(projectPath);
        var loadedModel = initialModel with
        {
            ProjectName = "my-project",
        };

        var store = Substitute.For<IProjectModelStore>();
        store.LoadAsync(initialModel, Arg.Any<CancellationToken>()).Returns(loadedModel);

        var projectNotLoadedException = new ProjectNotLoadedException("Project not found.");
        var projectModelPolicyEnforcer = Substitute.For<IProjectModelPolicyEnforcer>();
        projectModelPolicyEnforcer.When(stub => stub.ThrowIfNotLoaded(loadedModel)).Throw(projectNotLoadedException);

        var sut = Ctor([store], projectModelPolicyEnforcer);

        await Assert.ThatAsync(
            () => sut.LoadAsync(projectPath),
            Throws.Exception.EqualTo(projectNotLoadedException)
        );
    }

    [Test]
    public async Task SaveAsync_CallsSaveOnStoresWithDifferentHashAsync()
    {
        var model = new ProjectModel("/path")
        {
            ProjectName = "foo",
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        var store1 = Substitute.For<IProjectModelStore>();
        var store2 = Substitute.For<IProjectModelStore>();

        store1.GetContentHash(Arg.Any<ProjectModel>()).Returns("hash1", "hash2"); // different
        store2.GetContentHash(Arg.Any<ProjectModel>()).Returns("hash1", "hash1"); // same
        store1.LoadAsync(Arg.Any<ProjectModel>(), Arg.Any<CancellationToken>()).Returns(model);
        store2.LoadAsync(Arg.Any<ProjectModel>(), Arg.Any<CancellationToken>()).Returns(model);

        var sut = Ctor([store1, store2]);

        await sut.SaveAsync(model);

        await store1.Received(1).SaveAsync(model, Arg.Any<CancellationToken>());
        await store2.DidNotReceive().SaveAsync(model, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SaveAsync_DoesNotCallSaveIfHashesMatchAsync()
    {
        var model = new ProjectModel("/path")
        {
            ProjectName = "foo",
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        var store = Substitute.For<IProjectModelStore>();
        store.GetContentHash(Arg.Any<ProjectModel>()).Returns("hash");
        store.LoadAsync(Arg.Any<ProjectModel>(), Arg.Any<CancellationToken>()).Returns(model);

        var sut = Ctor([store]);

        await sut.SaveAsync(model);
        await store.DidNotReceive().SaveAsync(model, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SaveAsync_ThrowsIfCancelledAsync()
    {
        var model = new ProjectModel("/path")
        {
            ProjectName = "foo",
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        var store = Substitute.For<IProjectModelStore>();
        store.GetContentHash(Arg.Any<ProjectModel>()).Returns("hash1", "hash2");
        store.LoadAsync(Arg.Any<ProjectModel>(), Arg.Any<CancellationToken>()).Returns(model);

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        await cts.CancelAsync();

        var sut = Ctor([store]);

        await Assert.ThatAsync(
            () => sut.SaveAsync(model, cancellationToken),
            Throws.TypeOf<OperationCanceledException>()
        );
    }

    [Test]
    public async Task SaveAsync_CallsSaveOnMultipleStoresThatNeedUpdateAsync()
    {
        var model = new ProjectModel("/path")
        {
            ProjectName = "foo",
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var store1 = Substitute.For<IProjectModelStore>();
        var store2 = Substitute.For<IProjectModelStore>();

        store1.GetContentHash(Arg.Any<ProjectModel>()).Returns("hash1", "hash2");
        store2.GetContentHash(Arg.Any<ProjectModel>()).Returns("hash2", "hash3");
        store1.LoadAsync(Arg.Any<ProjectModel>(), cancellationToken).Returns(model);
        store2.LoadAsync(Arg.Any<ProjectModel>(), cancellationToken).Returns(model);

        var sut = Ctor([store1, store2]);

        await sut.SaveAsync(model, cancellationToken);

        await store1.Received(1).SaveAsync(model, cancellationToken);
        await store2.Received(1).SaveAsync(model, cancellationToken);
    }

    [Test]
    public async Task SaveAsync_DoesNothingIfNoStoresNeedUpdateAsync()
    {
        var model = new ProjectModel("/path")
        {
            ProjectName = "foo",
            KeystoneSync = new KeystoneSyncModel("template"),
        };

        var store1 = Substitute.For<IProjectModelStore>();
        var store2 = Substitute.For<IProjectModelStore>();

        store1.GetContentHash(Arg.Any<ProjectModel>()).Returns("hash");
        store2.GetContentHash(Arg.Any<ProjectModel>()).Returns("hash");
        store1.LoadAsync(Arg.Any<ProjectModel>(), Arg.Any<CancellationToken>()).Returns(model);
        store2.LoadAsync(Arg.Any<ProjectModel>(), Arg.Any<CancellationToken>()).Returns(model);

        var sut = Ctor([store1, store2]);

        await sut.SaveAsync(model);

        await store1.DidNotReceive().SaveAsync(model, Arg.Any<CancellationToken>());
        await store2.DidNotReceive().SaveAsync(model, Arg.Any<CancellationToken>());
    }
}
