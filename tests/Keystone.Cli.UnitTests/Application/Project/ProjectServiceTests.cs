using Keystone.Cli.Application.Data;
using Keystone.Cli.Application.GitHub;
using Keystone.Cli.Application.Project;
using Keystone.Cli.Domain;
using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.Domain.Project;
using Keystone.Cli.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;


namespace Keystone.Cli.UnitTests.Application.Project;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectServiceTests
{
    private static ProjectService Ctor(
        IGitHubService? gitHubService = null,
        ILogger<ProjectService>? logger = null,
        IProjectModelRepository? projectModelRepository = null
    )
        => new(
            gitHubService ?? Substitute.For<IGitHubService>(),
            logger ?? Substitute.For<ILogger<ProjectService>>(),
            projectModelRepository ?? Substitute.For<IProjectModelRepository>()
        );

    [Test]
    public async Task CreateNewAsync_LogsProjectCreationAsync()
    {
        const string projectName = "test-project";
        const string fullPath = "/path/to/project";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core";

        var templateTarget = new TemplateTargetModel(
            Name: "core",
            RepositoryUrl: new Uri(repositoryUrl)
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var projectModel = new ProjectModel(fullPath);
        var projectModelRepository = Substitute.For<IProjectModelRepository>();

        projectModelRepository.LoadAsync(fullPath, cancellationToken).Returns(
            _ => throw new ProjectNotLoadedException("Project not found."),
            _ => projectModel
        );

        var logger = new TestLogger<ProjectService>();
        var sut = Ctor(logger: logger, projectModelRepository: projectModelRepository);

        _ = await sut.CreateNewAsync(projectName, fullPath, templateTarget, includeGitFiles: false, cancellationToken);

        Assert.That(
            logger.CapturedLogEntries,
            Has.Some.Matches<LogEntry>(entry =>
                entry.Is(LogLevel.Information, $"Creating project '{projectName}' from {repositoryUrl} in {fullPath}")
            )
        );
    }

    [Test]
    public async Task CreateNewAsync_WithGitContentExcluded_CopiesRepositoryWithCorrectPredicateAsync()
    {
        const string projectName = "test-project";
        const string fullPath = "/path/to/project";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core";

        var templateTarget = new TemplateTargetModel(
            Name: "core",
            RepositoryUrl: new Uri(repositoryUrl),
            BranchName: "main"
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var projectModel = new ProjectModel(fullPath);
        var gitHubService = Substitute.For<IGitHubService>();
        var projectModelRepository = Substitute.For<IProjectModelRepository>();

        projectModelRepository.LoadAsync(fullPath, cancellationToken).Returns(
            _ => throw new ProjectNotLoadedException("Project not found."),
            _ => projectModel
        );

        var sut = Ctor(gitHubService: gitHubService, projectModelRepository: projectModelRepository);

        _ = await sut.CreateNewAsync(projectName, fullPath, templateTarget, includeGitFiles: false, cancellationToken);

        await gitHubService.Received(1).CopyPublicRepositoryAsync(
            templateTarget.RepositoryUrl,
            branchName: "main",
            destinationPath: fullPath,
            overwrite: true,
            predicate: EntryModelPolicies.ExcludeGitContent,
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task CreateNewAsync_WithGitContentIncluded_CopiesRepositoryWithCorrectPredicateAsync()
    {
        const string projectName = "test-project";
        const string fullPath = "/path/to/project";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core";

        var templateTarget = new TemplateTargetModel(
            Name: "core",
            RepositoryUrl: new Uri(repositoryUrl),
            BranchName: "develop"
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var projectModel = new ProjectModel(fullPath);
        var gitHubService = Substitute.For<IGitHubService>();
        var projectModelRepository = Substitute.For<IProjectModelRepository>();

        projectModelRepository.LoadAsync(fullPath, cancellationToken).Returns(
            _ => throw new ProjectNotLoadedException("Project not found."),
            _ => projectModel
        );

        var sut = Ctor(gitHubService: gitHubService, projectModelRepository: projectModelRepository);

        _ = await sut.CreateNewAsync(projectName, fullPath, templateTarget, includeGitFiles: true, cancellationToken);

        await gitHubService.Received(1).CopyPublicRepositoryAsync(
            templateTarget.RepositoryUrl,
            branchName: "develop",
            destinationPath: fullPath,
            overwrite: true,
            predicate: EntryModelPredicates.AcceptAll,
            cancellationToken: cancellationToken
        );
    }

    [Test]
    public async Task CreateNewAsync_ReturnsUpdatedProjectModelWithNewNameAsync()
    {
        const string projectName = "test-project";
        const string fullPath = "/path/to/project";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core";

        var templateTarget = new TemplateTargetModel(
            Name: "core",
            RepositoryUrl: new Uri(repositoryUrl)
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var originalModel = new ProjectModel(fullPath)
        {
            ProjectName = "template-name",
        };

        var expectedUpdatedModel = originalModel with
        {
            ProjectName = projectName,
        };

        var projectModelRepository = Substitute.For<IProjectModelRepository>();
        projectModelRepository.LoadAsync(fullPath, cancellationToken).Returns(
            _ => throw new ProjectNotLoadedException("Project not found"),
            _ => originalModel
        );

        var sut = Ctor(projectModelRepository: projectModelRepository);

        var actual = await sut.CreateNewAsync(projectName, fullPath, templateTarget, includeGitFiles: false, cancellationToken);

        Assert.That(actual, Is.EqualTo(expectedUpdatedModel));
        await projectModelRepository.Received(1).SaveAsync(expectedUpdatedModel, cancellationToken);
    }

    [Test]
    public async Task CreateNewAsync_WhenProjectAlreadyExists_ThrowsInvalidOperationExceptionAsync()
    {
        const string projectName = "new-project";
        const string fullPath = "/path/to/project";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core";

        var templateTarget = new TemplateTargetModel(
            Name: "core",
            RepositoryUrl: new Uri(repositoryUrl)
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var existingProject = new ProjectModel(fullPath)
        {
            ProjectName = "existing-project",
        };

        var projectModelRepository = Substitute.For<IProjectModelRepository>();
        projectModelRepository.LoadAsync(fullPath, cancellationToken).Returns(existingProject);

        var sut = Ctor(projectModelRepository: projectModelRepository);

        await Assert.ThatAsync(
            () => sut.CreateNewAsync(projectName, fullPath, templateTarget, includeGitFiles: false, cancellationToken),
            Throws.TypeOf<InvalidOperationException>()
                .With.Message.Contains($"A project already exists at '{fullPath}' with name '{existingProject.ProjectName}'")
        );
    }

    [Test]
    public async Task SwitchTemplateAsync_WhenProjectAlreadyUsesTemplate_ReturnsFalseAndLogsMessageAsync()
    {
        const string projectName = "existing-project";
        const string fullPath = "/path/to/project";
        const string templateName = "core";
        const string repositoryName = "template-core";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core";

        var templateTarget = new TemplateTargetModel(
            Name: templateName,
            RepositoryUrl: new Uri(repositoryUrl)
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var existingProject = new ProjectModel(fullPath)
        {
            ProjectName = projectName,
            KeystoneSync = new KeystoneSyncModel(repositoryName),
        };

        var projectModelRepository = Substitute.For<IProjectModelRepository>();
        projectModelRepository.LoadAsync(fullPath, cancellationToken).Returns(existingProject);

        var logger = new TestLogger<ProjectService>();
        var sut = Ctor(logger: logger, projectModelRepository: projectModelRepository);

        var actual = await sut.SwitchTemplateAsync(fullPath, templateTarget, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Is.False);
            Assert.That(
                logger.CapturedLogEntries,
                Has.Some.Matches<LogEntry>(entry =>
                    entry.Is(LogLevel.Information, $"Project '{projectName}' already uses {repositoryUrl} template, no change made")
                )
            );
        }
    }

    [Test]
    public async Task SwitchTemplateAsync_WhenProjectUsesNullKeystoneSync_SwitchesTemplateAsync()
    {
        const string projectName = "existing-project";
        const string fullPath = "/path/to/project";
        const string newTemplateName = "core-slim";
        const string newRepositoryName = "template-core-slim";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core-slim";

        var templateTarget = new TemplateTargetModel(
            Name: newTemplateName,
            RepositoryUrl: new Uri(repositoryUrl),
            BranchName: "main"
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var originalProject = new ProjectModel(fullPath)
        {
            ProjectName = projectName,
            KeystoneSync = null,
        };

        var refreshedProject = new ProjectModel(fullPath)
        {
            ProjectName = "template-default-name",
            KeystoneSync = new KeystoneSyncModel(newRepositoryName),
        };

        var gitHubService = Substitute.For<IGitHubService>();
        var projectModelRepository = Substitute.For<IProjectModelRepository>();
        projectModelRepository.LoadAsync(fullPath, cancellationToken).Returns(
            _ => originalProject,
            _ => refreshedProject
        );

        var logger = new TestLogger<ProjectService>();
        var sut = Ctor(gitHubService: gitHubService, logger: logger, projectModelRepository: projectModelRepository);

        var actual = await sut.SwitchTemplateAsync(fullPath, templateTarget, cancellationToken);

        Assert.That(actual, Is.True);

        await gitHubService.Received(1).CopyPublicRepositoryAsync(
            templateTarget.RepositoryUrl,
            branchName: "main",
            destinationPath: fullPath,
            overwrite: true,
            predicate: EntryModelPolicies.ExcludeGitAndUserContent,
            cancellationToken: cancellationToken
        );

        var expectedUpdatedModel = originalProject with
        {
            KeystoneSync = refreshedProject.KeystoneSync,
        };

        await projectModelRepository.Received(1).SaveAsync(expectedUpdatedModel, cancellationToken);
    }

    [Test]
    public async Task SwitchTemplateAsync_WhenProjectUsesDifferentTemplate_SwitchesTemplateAsync()
    {
        const string projectName = "existing-project";
        const string fullPath = "/path/to/project";
        const string newTemplateName = "core-slim";
        const string oldRepositoryName = "template-core";
        const string newRepositoryName = "template-core-slim";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core-slim";

        var templateTarget = new TemplateTargetModel(
            Name: newTemplateName,
            RepositoryUrl: new Uri(repositoryUrl),
            BranchName: "develop"
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var originalProject = new ProjectModel(fullPath)
        {
            ProjectName = projectName,
            KeystoneSync = new KeystoneSyncModel(oldRepositoryName),
        };

        var refreshedProject = new ProjectModel(fullPath)
        {
            ProjectName = "template-default-name",
            KeystoneSync = new KeystoneSyncModel(newRepositoryName),
        };

        var gitHubService = Substitute.For<IGitHubService>();
        var projectModelRepository = Substitute.For<IProjectModelRepository>();
        projectModelRepository.LoadAsync(fullPath, cancellationToken).Returns(
            _ => originalProject,
            _ => refreshedProject
        );

        var logger = new TestLogger<ProjectService>();
        var sut = Ctor(gitHubService: gitHubService, logger: logger, projectModelRepository: projectModelRepository);

        var actual = await sut.SwitchTemplateAsync(fullPath, templateTarget, cancellationToken);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Is.True);
            Assert.That(
                logger.CapturedLogEntries,
                Has.Some.Matches<LogEntry>(entry =>
                    entry.Is(LogLevel.Information, $"Switching project '{projectName}' to use {repositoryUrl} template in {fullPath}")
                )
            );
        }

        await gitHubService.Received(1).CopyPublicRepositoryAsync(
            templateTarget.RepositoryUrl,
            branchName: "develop",
            destinationPath: fullPath,
            overwrite: true,
            predicate: EntryModelPolicies.ExcludeGitAndUserContent,
            cancellationToken: cancellationToken
        );

        var expectedUpdatedModel = originalProject with
        {
            KeystoneSync = refreshedProject.KeystoneSync,
        };

        await projectModelRepository.Received(1).SaveAsync(expectedUpdatedModel, cancellationToken);
    }

    [Test]
    public async Task SwitchTemplateAsync_WhenProjectNotFound_ThrowsProjectNotLoadedExceptionAsync()
    {
        const string fullPath = "/path/to/nonexistent/project";
        const string repositoryUrl = "https://github.com/knight-owl-dev/template-core";

        var templateTarget = new TemplateTargetModel(
            Name: "core",
            RepositoryUrl: new Uri(repositoryUrl)
        );

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var exception = new ProjectNotLoadedException("Project not found.");
        var projectModelRepository = Substitute.For<IProjectModelRepository>();
        projectModelRepository
            .LoadAsync(fullPath, cancellationToken)
            .ThrowsAsync(exception);

        var sut = Ctor(projectModelRepository: projectModelRepository);

        await Assert.ThatAsync(
            () => sut.SwitchTemplateAsync(fullPath, templateTarget, cancellationToken),
            Throws.Exception.EqualTo(exception)
        );
    }
}
