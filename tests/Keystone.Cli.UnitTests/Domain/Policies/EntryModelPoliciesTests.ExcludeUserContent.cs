using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.UnitTests.Domain.FileSystem.TestData;


namespace Keystone.Cli.UnitTests.Domain.Policies;

public partial class EntryModelPoliciesTests
{
    private static class ExcludeUserContentTestCases
    {
        public static readonly TestCaseData<EntryModel>[] NonUserContentFromMinimal =
        [
            ..StandardProjectLayout.MinimalTemplateEntries
                .Where(entry => ! StandardProjectLayout.UserContentEntries.Contains(entry))
                .Select(entry => new TestCaseData<EntryModel>(entry)
                    {
                        ExpectedResult = true,
                        TestName = entry.RelativePath,
                    }
                ),
        ];

        public static readonly TestCaseData<EntryModel>[] NonUserContentFromFull =
        [
            ..StandardProjectLayout.FullTemplateEntries
                .Where(entry => ! StandardProjectLayout.UserContentEntries.Contains(entry))
                .Select(entry => new TestCaseData<EntryModel>(entry)
                    {
                        ExpectedResult = true,
                        TestName = entry.RelativePath,
                    }
                ),
        ];

        public static readonly TestCaseData<EntryModel>[] UserContentDirectories =
        [
            ..StandardProjectLayout.UserContentEntries
                .Where(entry => entry.Type == EntryType.Directory)
                .Select(entry => new TestCaseData<EntryModel>(entry)
                    {
                        ExpectedResult = false,
                        TestName = entry.RelativePath,
                    }
                ),
        ];

        public static readonly TestCaseData<EntryModel>[] UserContentFiles =
        [
            ..StandardProjectLayout.UserContentEntries
                .Where(entry => entry.Type == EntryType.File)
                .Select(entry => new TestCaseData<EntryModel>(entry)
                    {
                        ExpectedResult = false,
                        TestName = entry.RelativePath,
                    }
                ),
        ];

        public static readonly TestCaseData<EntryModel>[] EdgeCases =
        [
            new(EntryModel.Create("appendix-like-but-not/"))
            {
                ExpectedResult = true,
                TestName = "appendix-like-but-not/",
            },
            new(EntryModel.Create("appendix-like-but-not/file.md"))
            {
                ExpectedResult = true,
                TestName = "appendix-like-but-not/file.md",
            },
            new(EntryModel.Create("sub/nested/appendix/"))
            {
                ExpectedResult = true,
                TestName = "sub/nested/appendix/",
            },
            new(EntryModel.Create("artifacts-backup/output.pdf"))
            {
                ExpectedResult = true,
                TestName = "artifacts-backup/output.pdf",
            },
            new(EntryModel.Create("chapters-backup/file.md"))
            {
                ExpectedResult = true,
                TestName = "chapters-backup/file.md",
            },
            new(EntryModel.Create("non-user-content.md"))
            {
                ExpectedResult = true,
                TestName = "non-user-content.md",
            },
        ];
    }

    [TestCaseSource(typeof(ExcludeUserContentTestCases), nameof(ExcludeUserContentTestCases.NonUserContentFromMinimal))]
    public bool ExcludeUserContent_NonUserContentFromMinimal(EntryModel entry)
        => EntryModelPolicies.ExcludeUserContent(entry);

    [TestCaseSource(typeof(ExcludeUserContentTestCases), nameof(ExcludeUserContentTestCases.NonUserContentFromFull))]
    public bool ExcludeUserContent_NonUserContentFromFull(EntryModel entry)
        => EntryModelPolicies.ExcludeUserContent(entry);

    [TestCaseSource(typeof(ExcludeUserContentTestCases), nameof(ExcludeUserContentTestCases.UserContentDirectories))]
    public bool ExcludeUserContent_UserContentDirectories(EntryModel entry)
        => EntryModelPolicies.ExcludeUserContent(entry);

    [TestCaseSource(typeof(ExcludeUserContentTestCases), nameof(ExcludeUserContentTestCases.UserContentFiles))]
    public bool ExcludeUserContent_UserContentFiles(EntryModel entry)
        => EntryModelPolicies.ExcludeUserContent(entry);

    [TestCaseSource(typeof(ExcludeUserContentTestCases), nameof(ExcludeUserContentTestCases.EdgeCases))]
    public bool ExcludeUserContent_EdgeCases(EntryModel entry)
        => EntryModelPolicies.ExcludeUserContent(entry);

    [Test]
    public void ExcludeUserContent_WithUnknownEntryType_ReturnsTrue()
    {
        var entry = new EntryModel((EntryType) 999, "unknown", "unknown");

        var result = EntryModelPolicies.ExcludeUserContent(entry);

        Assert.That(result, Is.True);
    }
}
