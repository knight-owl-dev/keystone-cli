using Keystone.Cli.Application.Data;
using Keystone.Cli.Application.Project;
using Keystone.Cli.Domain.Project;
using Keystone.Cli.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Project;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectServiceTests
{
    private static ProjectService Ctor(
        ILogger<ProjectService>? logger = null,
        IProjectModelRepository? projectModelRepository = null
    )
        => new(
            logger ?? Substitute.For<ILogger<ProjectService>>(),
            projectModelRepository ?? Substitute.For<IProjectModelRepository>()
        );

    [Test]
    public async Task SetProjectNameAsync_LoadsProjectAndSavesWithUpdatedNameAsync()
    {
        const string projectRoot = "/path/to/project";
        const string newName = "new-project-name";

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var originalModel = new ProjectModel(projectRoot)
        {
            ProjectName = "old-name",
        };

        var expectedUpdatedModel = originalModel with
        {
            ProjectName = newName,
        };

        var projectModelRepository = Substitute.For<IProjectModelRepository>();
        projectModelRepository.LoadAsync(projectRoot, cancellationToken).Returns(originalModel);

        var sut = Ctor(projectModelRepository: projectModelRepository);

        await sut.SetProjectNameAsync(projectRoot, newName, cancellationToken);

        await projectModelRepository.Received(1).LoadAsync(projectRoot, cancellationToken);
        await projectModelRepository.Received(1).SaveAsync(expectedUpdatedModel, cancellationToken);
    }

    [Test]
    public async Task SetProjectNameAsync_LogsProjectNameChangeAsync()
    {
        const string projectRoot = "/path/to/project";
        const string oldName = "old-project-name";
        const string newName = "new-project-name";

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var originalModel = new ProjectModel(projectRoot)
        {
            ProjectName = oldName,
        };

        var projectModelRepository = Substitute.For<IProjectModelRepository>();
        projectModelRepository.LoadAsync(projectRoot, cancellationToken).Returns(originalModel);

        var logger = new TestLogger<ProjectService>();
        var sut = Ctor(logger: logger, projectModelRepository: projectModelRepository);

        await sut.SetProjectNameAsync(projectRoot, newName, cancellationToken);

        Assert.That(
            logger.CapturedLogEntries,
            Has.Some.Matches<LogEntry>(entry =>
                entry.Is(LogLevel.Information, $"Updated the project name from '{oldName}' to '{newName}' at '{projectRoot}'")
            )
        );
    }
}
