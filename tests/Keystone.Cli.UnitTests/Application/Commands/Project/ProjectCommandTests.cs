using Keystone.Cli.Application.Commands.Project;


namespace Keystone.Cli.UnitTests.Application.Commands.Project;

[TestFixture, Parallelizable(ParallelScope.All)]
public class ProjectCommandTests
{
    private static ProjectCommand Ctor()
        => new();

    [Test]
    public async Task SwitchTemplateAsync_NoChanges_ReturnsFalseAsync()
    {
        const string newTemplateName = "new-template";
        const string projectPath = "/path/to/project";

        var sut = Ctor();

        var actual = await sut.SwitchTemplateAsync(newTemplateName, projectPath, CancellationToken.None);

        Assert.That(actual, Is.False);
    }
}
