using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.Domain.Policies;


namespace Keystone.Cli.UnitTests.Domain.Policies;

[TestFixture, Parallelizable(ParallelScope.All)]
public class EntryModelPoliciesTests
{
    /// <summary>
    /// The project content test cases always included.
    /// </summary>
    private static readonly TestCaseData<EntryModel>[] ProjectContentTestCases =
    [
        new(EntryModel.Create(".docker/"))
        {
            ExpectedResult = true,
            TestName = "Includes .docker directory",
        },
        new(EntryModel.Create(".docker/docker-compose.yml"))
        {
            ExpectedResult = true,
            TestName = "Includes .docker/docker-compose.yml",
        },
        new(EntryModel.Create(".keystone/"))
        {
            ExpectedResult = true,
            TestName = "Includes .keystone directory",
        },
        new(EntryModel.Create(".keystone/sync.json"))
        {
            ExpectedResult = true,
            TestName = "Includes .keystone/sync.json file",
        },
        new(EntryModel.Create(".licenses/"))
        {
            ExpectedResult = true,
            TestName = "Includes .licenses directory",
        },
        new(EntryModel.Create(".licenses/Keystone.md"))
        {
            ExpectedResult = true,
            TestName = "Includes .licenses/Keystone.md file",
        },
        new(EntryModel.Create(".licenses/Pandoc.md"))
        {
            ExpectedResult = true,
            TestName = "Includes .licenses/Pandoc.md file",
        },
        new(EntryModel.Create("appendix/"))
        {
            ExpectedResult = true,
            TestName = "Includes appendix directory",
        },
        new(EntryModel.Create("artifacts/"))
        {
            ExpectedResult = true,
            TestName = "Includes artifacts directory",
        },
        new(EntryModel.Create("assets/"))
        {
            ExpectedResult = true,
            TestName = "Includes assets directory",
        },
        new(EntryModel.Create("chapters/"))
        {
            ExpectedResult = true,
            TestName = "Includes chapters directory",
        },
        new(EntryModel.Create("drafts/"))
        {
            ExpectedResult = true,
            TestName = "Includes drafts directory",
        },
        new(EntryModel.Create("research/"))
        {
            ExpectedResult = true,
            TestName = "Includes research directory",
        },
        new(EntryModel.Create(".editorconfig"))
        {
            ExpectedResult = true,
            TestName = "Includes .editorconfig file",
        },
        new(EntryModel.Create(".dockerignore"))
        {
            ExpectedResult = true,
            TestName = "Includes .dockerignore file",
        },
        new(EntryModel.Create(".env"))
        {
            ExpectedResult = true,
            TestName = "Includes .env file",
        },
        new(EntryModel.Create("Makefile"))
        {
            ExpectedResult = true,
            TestName = "Includes Makefile",
        },
        new(EntryModel.Create("NOTICE.md"))
        {
            ExpectedResult = true,
            TestName = "Includes NOTICE.md file",
        },
        new(EntryModel.Create("pandoc.yaml"))
        {
            ExpectedResult = true,
            TestName = "Includes pandoc.yaml file",
        },
        new(EntryModel.Create("publish.txt"))
        {
            ExpectedResult = true,
            TestName = "Includes publish.txt file",
        },
        new(EntryModel.Create("README.md"))
        {
            ExpectedResult = true,
            TestName = "Includes README.md file",
        },
    ];

    /// <summary>
    /// The Git content test cases always excluded.
    /// </summary>
    private static readonly TestCaseData<EntryModel>[] GitContentTestCases =
    [
        new(EntryModel.Create(".gitignore"))
        {
            ExpectedResult = false,
            TestName = "Excludes .gitignore file",
        },
        new(EntryModel.Create(".gitattributes"))
        {
            ExpectedResult = false,
            TestName = "Excludes .gitattributes file",
        },
        new(EntryModel.Create(".gitkeep"))
        {
            ExpectedResult = false,
            TestName = "Excludes .gitkeep file",
        },
        new(EntryModel.Create(".gitmodules"))
        {
            ExpectedResult = false,
            TestName = "Excludes .gitmodules file",
        },
        new(EntryModel.Create(".git/"))
        {
            ExpectedResult = false,
            TestName = "Excludes .git directory",
        },
        new(EntryModel.Create("appendix/.gitkeep"))
        {
            ExpectedResult = false,
            TestName = "Excludes appendix/.gitkeep file",
        },
        new(EntryModel.Create("artifacts/.gitkeep"))
        {
            ExpectedResult = false,
            TestName = "Excludes artifacts/.gitkeep file",
        },
        new(EntryModel.Create("assets/.gitkeep"))
        {
            ExpectedResult = false,
            TestName = "Excludes assets/.gitkeep file",
        },
        new(EntryModel.Create("chapters/.gitkeep"))
        {
            ExpectedResult = false,
            TestName = "Excludes chapters/.gitkeep file",
        },
        new(EntryModel.Create("drafts/.gitkeep"))
        {
            ExpectedResult = false,
            TestName = "Excludes drafts/.gitkeep file",
        },
        new(EntryModel.Create("research/.gitkeep"))
        {
            ExpectedResult = false,
            TestName = "Excludes research/.gitkeep file",
        },
    ];

    [TestCaseSource(nameof(GitContentTestCases))]
    [TestCaseSource(nameof(ProjectContentTestCases))]
    public bool ExcludeGitContent_ReturnsFalse(EntryModel entry)
        => EntryModelPolicies.ExcludeGitContent(entry);
}
