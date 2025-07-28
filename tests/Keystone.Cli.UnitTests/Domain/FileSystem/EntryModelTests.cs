using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.UnitTests.Domain.FileSystem;

[TestFixture, Parallelizable(ParallelScope.All)]
public class EntryModelTests
{
    [Test]
    public void Create_ForFile_InRoot_ReturnsFileEntry()
    {
        const string fileName = "C.txt";

        var expected = new EntryModel(EntryType.File, fileName, fileName);
        var actual = EntryModel.Create(relativePath: fileName);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Create_ForFile_InSubDirectory_ReturnsFileEntry()
    {
        const string fileName = "C.txt";
        const string relativePath = $"A/{fileName}";

        var expected = new EntryModel(EntryType.File, fileName, relativePath);
        var actual = EntryModel.Create(relativePath);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Create_ForDirectory_InRoot_ReturnsDirectoryEntry()
    {
        const string relativePath = "A/";

        var expected = new EntryModel(EntryType.Directory, Name: string.Empty, relativePath);
        var actual = EntryModel.Create(relativePath);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Create_ForDirectory_InSubDirectory_ReturnsDirectoryEntry()
    {
        const string relativePath = "A/B/";

        var expected = new EntryModel(EntryType.Directory, Name: string.Empty, relativePath);
        var actual = EntryModel.Create(relativePath);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetFullPath_ForFile_InRoot_ReturnsFullPath()
    {
        const string rootPath = "./test";
        const string fileName = "C.txt";

        var entry = EntryModel.Create(fileName);
        var expected = Path.Combine(rootPath, fileName);

        var actual = entry.GetFullPath(rootPath);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetFullPath_ForFile_InSubDirectory_ReturnsFullPath()
    {
        const string rootPath = "./test";
        const string fileName = "C.txt";
        const string relativePath = $"A/{fileName}";

        var entry = EntryModel.Create(relativePath);
        var expected = Path.Combine(rootPath, relativePath);

        var actual = entry.GetFullPath(rootPath);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetFullPath_ForDirectory_InRoot_ReturnsFullPath()
    {
        const string rootPath = "./test";
        const string relativePath = "A/";

        var entry = EntryModel.Create(relativePath);
        var expected = Path.Combine(rootPath, relativePath);

        var actual = entry.GetFullPath(rootPath);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetFullPath_ForDirectory_InSubDirectory_ReturnsFullPath()
    {
        const string rootPath = "./test";
        const string relativePath = "A/B/";

        var entry = EntryModel.Create(relativePath);
        var expected = Path.Combine(rootPath, relativePath);

        var actual = entry.GetFullPath(rootPath);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetDirectoryName_ForFile_InRoot_ReturnsEmpty()
    {
        const string relativePath = "C.txt";

        var entry = EntryModel.Create(relativePath);
        var actual = entry.GetDirectoryName();

        Assert.That(actual, Is.Empty);
    }

    [Test]
    public void GetDirectoryName_ForFile_InSubDirectory_ReturnsDirectoryName()
    {
        const string relativePath = "A/B/C.txt";
        const string expectedDirectoryName = "A/B";

        var entry = EntryModel.Create(relativePath);
        var actual = entry.GetDirectoryName();

        Assert.That(actual, Is.EqualTo(expectedDirectoryName));
    }

    [Test]
    public void GetDirectoryName_ForDirectory_InRoot_ReturnsSelf()
    {
        const string relativePath = "A/";
        const string expectedDirectoryName = "A";

        var entry = EntryModel.Create(relativePath);
        var actual = entry.GetDirectoryName();

        Assert.That(actual, Is.EqualTo(expectedDirectoryName));
    }

    [Test]
    public void GetDirectoryName_ForDirectory_InSubDirectory_ReturnsSelf()
    {
        const string relativePath = "A/B/";
        const string expectedDirectoryName = "A/B";

        var entry = EntryModel.Create(relativePath);
        var actual = entry.GetDirectoryName();

        Assert.That(actual, Is.EqualTo(expectedDirectoryName));
    }

    [Test]
    public void GetDirectoryName_IsRootEntry_ThrowsInvalidOperationException()
    {
        const string relativePath = "/";

        var entry = EntryModel.Create(relativePath);

        Assert.That(
            () => entry.GetDirectoryName(),
            Throws.InvalidOperationException.With.Message.EqualTo("Cannot get directory name for the root entry.")
        );
    }
}
