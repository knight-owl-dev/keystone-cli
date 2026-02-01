using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.UnitTests.Domain.FileSystem;

[TestFixture, Parallelizable(ParallelScope.All)]
public class EntryModelSortPolicyTests
{
    [Test]
    public void DirectoriesFirst_WhenAllFiles_ReturnsFilesInOrder()
    {
        EntryModel[] entries = [EntryModel.Create("C.txt"), EntryModel.Create("A.txt"), EntryModel.Create("B.txt")];
        EntryModel[] expected = [EntryModel.Create("A.txt"), EntryModel.Create("B.txt"), EntryModel.Create("C.txt")];

        EntryModel[] actual = [.. EntryModelSortPolicy.DirectoriesFirst(entries)];

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void DirectoriesFirst_WhenMixedWithDirectories_ReturnsDirectoriesFirstFollowedByFilesInOrder()
    {
        EntryModel[] entries =
        [
            EntryModel.Create("3.txt"),
            EntryModel.Create("2.txt"),
            EntryModel.Create("1.txt"),
            EntryModel.Create("C/"),
            EntryModel.Create("A/"),
            EntryModel.Create("B/"),
        ];

        EntryModel[] expected =
        [
            EntryModel.Create("A/"),
            EntryModel.Create("B/"),
            EntryModel.Create("C/"),
            EntryModel.Create("1.txt"),
            EntryModel.Create("2.txt"),
            EntryModel.Create("3.txt"),
        ];

        EntryModel[] actual = [.. EntryModelSortPolicy.DirectoriesFirst(entries)];

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void DirectoriesFirst_IsCaseSensitive()
    {
        EntryModel[] entries = [EntryModel.Create("a.txt"), EntryModel.Create("A.txt"), EntryModel.Create("b.txt"), EntryModel.Create("B.txt")];
        EntryModel[] expected = [EntryModel.Create("A.txt"), EntryModel.Create("B.txt"), EntryModel.Create("a.txt"), EntryModel.Create("b.txt")];

        EntryModel[] actual = [.. EntryModelSortPolicy.DirectoriesFirst(entries)];

        Assert.That(actual, Is.EqualTo(expected));
    }
}
