using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.UnitTests.Domain.FileSystem.TestData;


namespace Keystone.Cli.UnitTests.Domain.Policies;

public partial class EntryModelPoliciesTests
{
    private static class ExcludeGitAndUserContentTestCases
    {
        public static readonly TestCaseData<EntryModel>[] AcceptedContent =
        [
            ..StandardProjectLayout.MinimalTemplateEntries
                .Where(entry => ! StandardProjectLayout.GitEntries.Contains(entry) && ! StandardProjectLayout.UserContentEntries.Contains(entry))
                .Select(entry => new TestCaseData<EntryModel>(entry)
                    {
                        ExpectedResult = true,
                        TestName = entry.RelativePath,
                    }
                ),
            ..StandardProjectLayout.FullTemplateEntries
                .Where(entry => ! StandardProjectLayout.GitEntries.Contains(entry) && ! StandardProjectLayout.UserContentEntries.Contains(entry))
                .Select(entry => new TestCaseData<EntryModel>(entry)
                    {
                        ExpectedResult = true,
                        TestName = entry.RelativePath,
                    }
                ),
        ];

        public static readonly TestCaseData<EntryModel>[] GitContent =
        [
            ..StandardProjectLayout.GitEntries.Select(entry
                => new TestCaseData<EntryModel>(entry)
                {
                    ExpectedResult = false,
                    TestName = $"git:{entry.RelativePath}",
                }
            ),
        ];

        public static readonly TestCaseData<EntryModel>[] UserContent =
        [
            ..StandardProjectLayout.UserContentEntries.Select(entry
                => new TestCaseData<EntryModel>(entry)
                {
                    ExpectedResult = false,
                    TestName = $"user:{entry.RelativePath}",
                }
            ),
        ];

        public static readonly TestCaseData<EntryModel>[] MixedEdgeCases =
        [
            new(EntryModel.Create(".keystone/"))
            {
                ExpectedResult = true,
                TestName = ".keystone/ (should be accepted)",
            },
            new(EntryModel.Create(".keystone/sync.json"))
            {
                ExpectedResult = true,
                TestName = ".keystone/sync.json (should be accepted)",
            },
            new(EntryModel.Create(".pandoc/"))
            {
                ExpectedResult = true,
                TestName = ".pandoc/ (should be accepted)",
            },
            new(EntryModel.Create("Makefile"))
            {
                ExpectedResult = true,
                TestName = "Makefile (should be accepted)",
            },
            new(EntryModel.Create("README.md"))
            {
                ExpectedResult = true,
                TestName = "README.md (should be accepted)",
            },
        ];
    }

    [TestCaseSource(typeof(ExcludeGitAndUserContentTestCases), nameof(ExcludeGitAndUserContentTestCases.AcceptedContent))]
    public bool ExcludeGitAndUserContent_AcceptedContent(EntryModel entry)
        => EntryModelPolicies.ExcludeGitAndUserContent(entry);

    [TestCaseSource(typeof(ExcludeGitAndUserContentTestCases), nameof(ExcludeGitAndUserContentTestCases.GitContent))]
    public bool ExcludeGitAndUserContent_GitContent(EntryModel entry)
        => EntryModelPolicies.ExcludeGitAndUserContent(entry);

    [TestCaseSource(typeof(ExcludeGitAndUserContentTestCases), nameof(ExcludeGitAndUserContentTestCases.UserContent))]
    public bool ExcludeGitAndUserContent_UserContent(EntryModel entry)
        => EntryModelPolicies.ExcludeGitAndUserContent(entry);

    [TestCaseSource(typeof(ExcludeGitAndUserContentTestCases), nameof(ExcludeGitAndUserContentTestCases.MixedEdgeCases))]
    public bool ExcludeGitAndUserContent_MixedEdgeCases(EntryModel entry)
        => EntryModelPolicies.ExcludeGitAndUserContent(entry);

    [Test]
    public void ExcludeGitAndUserContent_WithGitFileInUserContentDirectory_ReturnsFalse()
    {
        var entry = EntryModel.Create("appendix/.gitkeep");

        var actual = EntryModelPolicies.ExcludeGitAndUserContent(entry);

        Assert.That(actual, Is.False, "Should exclude files that are both git-related AND in user content directories");
    }

    [Test]
    public void ExcludeGitAndUserContent_WithUnknownEntryType_ReturnsTrue()
    {
        var entry = new EntryModel((EntryType) 999, "unknown", "unknown");

        var actual = EntryModelPolicies.ExcludeGitAndUserContent(entry);

        Assert.That(actual, Is.True);
    }
}
