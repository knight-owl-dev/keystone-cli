using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.UnitTests.Domain.FileSystem;

[TestFixture, Parallelizable(ParallelScope.All)]
public class EntryModelPathAnalyzerTests
{
    private static IEnumerable<TestCaseData> IsDirectChildOf_File_TestCases()
    {
        yield return new TestCaseData("artifacts/file.pdf", "artifacts")
            .Returns(true)
            .SetName("IsDirectChildOf_FileInTargetDirectory_ReturnsTrue");

        yield return new TestCaseData("artifacts/sub/file.pdf", "artifacts")
            .Returns(true)
            .SetName("IsDirectChildOf_FileInTargetSubdirectory_ReturnsTrue");

        yield return new TestCaseData("artifacts-backup/file.pdf", "artifacts")
            .Returns(false)
            .SetName("IsDirectChildOf_FileInSimilarDirectory_ReturnsFalse");

        yield return new TestCaseData("sub/artifacts/file.pdf", "artifacts")
            .Returns(false)
            .SetName("IsDirectChildOf_FileInNestedDirectory_ReturnsFalse");

        yield return new TestCaseData("appendix/chapter1.md", "artifacts")
            .Returns(false)
            .SetName("IsDirectChildOf_FileInDifferentDirectory_ReturnsFalse");

        yield return new TestCaseData("README.md", "artifacts")
            .Returns(false)
            .SetName("IsDirectChildOf_FileInRoot_ReturnsFalse");
    }

    [TestCaseSource(nameof(IsDirectChildOf_File_TestCases))]
    public bool IsDirectChildOf_WithFile(string relativePath, string directoryName)
    {
        var entry = EntryModel.Create(relativePath);

        return entry.IsDirectChildOf(directoryName);
    }

    private static IEnumerable<TestCaseData> IsDirectChildOf_Directory_TestCases()
    {
        yield return new TestCaseData("artifacts/", "artifacts")
            .Returns(false)
            .SetName("IsDirectChildOf_Directory_ReturnsFalse");

        yield return new TestCaseData("artifacts-backup/", "artifacts")
            .Returns(false)
            .SetName("IsDirectChildOf_SimilarDirectory_ReturnsFalse");
    }

    [TestCaseSource(nameof(IsDirectChildOf_Directory_TestCases))]
    public bool IsDirectChildOf_WithDirectory(string relativePath, string directoryName)
    {
        var entry = EntryModel.Create(relativePath);

        return entry.IsDirectChildOf(directoryName);
    }

    [Test]
    public void IsInAnyDirectory_WithFileInOneOfMultipleDirectories_ReturnsTrue()
    {
        var entry = EntryModel.Create("chapters/introduction.md");
        var directories = new[] { "appendix", "chapters", "artifacts" };

        var result = entry.IsInAnyDirectory(directories);

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsInAnyDirectory_WithFileInNoneOfDirectories_ReturnsFalse()
    {
        var entry = EntryModel.Create("docs/readme.md");
        var directories = new[] { "appendix", "chapters", "artifacts" };

        var result = entry.IsInAnyDirectory(directories);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsInAnyDirectory_WithFileInSimilarlyNamedDirectory_ReturnsFalse()
    {
        var entry = EntryModel.Create("chapters-backup/file.md");
        var directories = new[] { "chapters" };

        var result = entry.IsInAnyDirectory(directories);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsInAnyDirectory_WithDirectory_ReturnsFalse()
    {
        var entry = EntryModel.Create("chapters/");
        var directories = new[] { "chapters" };

        var result = entry.IsInAnyDirectory(directories);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsInAnyDirectory_WithEmptyDirectoryList_ReturnsFalse()
    {
        var entry = EntryModel.Create("chapters/file.md");
        var directories = Array.Empty<string>();

        var result = entry.IsInAnyDirectory(directories);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsDirectChildOf_WithNullDirectoryName_ThrowsArgumentNullException()
    {
        var entry = EntryModel.Create("file.txt");

        Assert.That(
            () => entry.IsDirectChildOf(directoryName: null!),
            Throws.TypeOf<ArgumentNullException>()
        );
    }

    [Test]
    public void IsInAnyDirectory_WithNullDirectoryNames_ThrowsArgumentNullException()
    {
        var entry = EntryModel.Create("file.txt");

        Assert.That(
            () => entry.IsInAnyDirectory(directoryNames: null!),
            Throws.TypeOf<ArgumentNullException>()
        );
    }

    [Test]
    public void IsDirectChildOf_WithEmptyDirectoryName_ReturnsFalse()
    {
        var entry = EntryModel.Create("file.txt");

        var result = entry.IsDirectChildOf("");

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsInAnyDirectory_WithDirectoryContainingEmptyString_HandlesProperly()
    {
        var entry = EntryModel.Create("file.txt");
        var directories = new[] { "", "chapters" };

        var result = entry.IsInAnyDirectory(directories);

        Assert.That(result, Is.False);
    }
}
