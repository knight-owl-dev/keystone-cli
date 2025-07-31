using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.Domain.Policies;
using Keystone.Cli.UnitTests.Domain.FileSystem.TestData;


namespace Keystone.Cli.UnitTests.Domain.Policies;

partial class EntryModelPoliciesTests
{
    private static class ExcludeGitContentTestCases
    {
        public static readonly TestCaseData<EntryModel>[] MinimalProjectContent =
        [
            ..StandardProjectLayout.MinimalTemplateEntries.Select(entry
                => new TestCaseData<EntryModel>(entry)
                {
                    ExpectedResult = ! StandardProjectLayout.GitEntries.Contains(entry),
                    TestName = entry.RelativePath,
                }
            ),
        ];

        public static readonly TestCaseData<EntryModel>[] FullProjectContent =
        [
            ..StandardProjectLayout.FullTemplateEntries.Select(entry
                => new TestCaseData<EntryModel>(entry)
                {
                    ExpectedResult = ! StandardProjectLayout.GitEntries.Contains(entry),
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
                    TestName = entry.RelativePath,
                }
            ),
        ];
    }

    [TestCaseSource(typeof(ExcludeGitContentTestCases), nameof(ExcludeGitContentTestCases.MinimalProjectContent))]
    public bool ExcludeGitContent_MinimalProjectContent(EntryModel entry)
        => EntryModelPolicies.ExcludeGitContent(entry);

    [TestCaseSource(typeof(ExcludeGitContentTestCases), nameof(ExcludeGitContentTestCases.FullProjectContent))]
    public bool ExcludeGitContent_FullProjectContent(EntryModel entry)
        => EntryModelPolicies.ExcludeGitContent(entry);

    [TestCaseSource(typeof(ExcludeGitContentTestCases), nameof(ExcludeGitContentTestCases.GitContent))]
    public bool ExcludeGitContent_GitContent(EntryModel entry)
        => EntryModelPolicies.ExcludeGitContent(entry);
}
