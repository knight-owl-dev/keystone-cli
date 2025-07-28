using System.Collections.Immutable;
using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.UnitTests.Domain.FileSystem;

[TestFixture, Parallelizable(ParallelScope.All)]
public class EntryNodeTests
{
    [Test]
    public void ToString_ForFileEntry_ReturnsRelativePath()
    {
        var entry = EntryModel.Create("file.txt");
        var sut = new EntryNode(EntryModel.Create("file.txt"));

        var actual = sut.ToString();

        Assert.That(actual, Is.EqualTo(entry.RelativePath));
    }

    [Test]
    public void ToString_ForDirectoryEntry_ReturnsRelativePath()
    {
        var entry = EntryModel.Create("A/");
        var sut = new EntryNode(entry);

        var actual = sut.ToString();

        Assert.That(actual, Is.EqualTo(entry.RelativePath));
    }

    [Test]
    public void ToString_AggregatesAllRelativePaths()
    {
        const string expected = """
            A/
            A/file1.txt
            A/file2.txt
            A/B/
            A/B/file1.txt
            A/B/C/
            """;

        var sut = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")))
            .AddChild(new EntryNode(EntryModel.Create("A/file2.txt")))
            .AddChild(
                new EntryNode(EntryModel.Create("A/B/"))
                    .AddChild(new EntryNode(EntryModel.Create("A/B/file1.txt")))
                    .AddChild(new EntryNode(EntryModel.Create("A/B/C/")))
            );

        var actual = sut.ToString();

        Assert.That(actual, Is.EqualTo(expected).IgnoreWhiteSpace);
    }

    [Test]
    public void Equals_ForFileEntry_WhenEqual_ReturnsTrue()
    {
        var a = new EntryNode(EntryModel.Create("file1.txt"));
        var b = new EntryNode(EntryModel.Create("file1.txt"));

        var actual = a.Equals(b);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void Equals_ForFileEntry_WhenNotEqual_ReturnsFalse()
    {
        var a = new EntryNode(EntryModel.Create("file1.txt"));
        var b = new EntryNode(EntryModel.Create("file2.txt"));

        var actual = a.Equals(b);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void Equals_ForDirectoryEntry_WhenEqual_NotEmpty_ReturnsTrue()
    {
        var a = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")));

        var b = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")));

        var actual = a.Equals(b);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void Equals_ForDirectoryEntry_WhenEqual_Empty_ReturnsTrue()
    {
        var a = new EntryNode(EntryModel.Create("A/"));
        var b = new EntryNode(EntryModel.Create("A/"));

        var actual = a.Equals(b);

        Assert.That(actual, Is.True);
    }

    [Test]
    public void Equals_ForDirectoryEntry_WhenNotEqual_NotEmpty_ReturnsFalse()
    {
        var a = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")));

        var b = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")))
            .AddChild(new EntryNode(EntryModel.Create("A/file2.txt")));

        var actual = a.Equals(b);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void Equals_ForDirectoryEntry_WhenNotEqual_Empty_ReturnsFalse()
    {
        var a = new EntryNode(EntryModel.Create("A/"));

        var b = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")));

        var actual = a.Equals(b);

        Assert.That(actual, Is.False);
    }

    [Test]
    public void AddChild_ForFileEntry_ThrowsInvalidOperationException()
    {
        var sut = new EntryNode(EntryModel.Create("file.txt"));

        Assert.That(
            () => sut.AddChild(new EntryNode(EntryModel.Create("child.txt"))),
            Throws.InvalidOperationException.With.Message.EqualTo("Cannot add children to a file entry.")
        );
    }

    [Test]
    public void AddChildren_ForFileEntry_ThrowsInvalidOperationException()
    {
        var sut = new EntryNode(EntryModel.Create("file.txt"));

        Assert.That(
            () => sut.AddChildren([new EntryNode(EntryModel.Create("child.txt"))]),
            Throws.InvalidOperationException.With.Message.EqualTo("Cannot add children to a file entry.")
        );
    }

    [Test]
    public void AddChildren_AddsChildrenToDirectoryEntry()
    {
        var dir = EntryModel.Create("A/");
        var file1 = EntryModel.Create("A/file1.txt");
        var file2 = EntryModel.Create("A/file2.txt");
        var file3 = EntryModel.Create("A/file3.txt");

        var expected = new EntryNode(dir)
            .AddChild(new EntryNode(file1))
            .AddChild(new EntryNode(file2))
            .AddChild(new EntryNode(file3));

        var actual = new EntryNode(EntryModel.Create("A/"))
            .AddChildren([new EntryNode(file1), new EntryNode(file2)])
            .AddChildren([new EntryNode(file3)]);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Aggregate_ReturnsFromResultSelector()
    {
        var sut = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")))
            .AddChild(new EntryNode(EntryModel.Create("A/file2.txt")))
            .AddChild(
                new EntryNode(EntryModel.Create("A/B/"))
                    .AddChild(new EntryNode(EntryModel.Create("A/B/file1.txt")))
                    .AddChild(new EntryNode(EntryModel.Create("A/B/C/")))
            );

        var actual = sut.Aggregate(
            ImmutableList.CreateBuilder<string>(),
            _ => true,
            (acc, entry) =>
            {
                acc.Add(entry.RelativePath);

                return acc;
            },
            acc => acc.ToImmutable()
        );

        var expected = ImmutableList.Create(
            "A/",
            "A/file1.txt",
            "A/file2.txt",
            "A/B/",
            "A/B/file1.txt",
            "A/B/C/"
        );

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Aggregate_EverythingRejected_ReturnsSeed()
    {
        var sut = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")));

        var seed = ImmutableList<string>.Empty;

        var actual = sut.Aggregate(
            seed,
            _ => false,
            (acc, entry) => acc.Add(entry.RelativePath)
        );

        Assert.That(actual, Is.SameAs(seed));
    }

    [Test]
    public void Aggregate_ReturnsAccumulator()
    {
        var sut = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")))
            .AddChild(new EntryNode(EntryModel.Create("A/file2.txt")))
            .AddChild(
                new EntryNode(EntryModel.Create("A/B/"))
                    .AddChild(new EntryNode(EntryModel.Create("A/B/file1.txt")))
                    .AddChild(new EntryNode(EntryModel.Create("A/B/C/")))
            );

        var actual = sut.Aggregate(
            ImmutableList<string>.Empty,
            _ => true,
            (acc, entry) => acc.Add(entry.RelativePath)
        );

        var expected = ImmutableList.Create(
            "A/",
            "A/file1.txt",
            "A/file2.txt",
            "A/B/",
            "A/B/file1.txt",
            "A/B/C/"
        );

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Aggregate_RejectingDirectory_IgnoresAllChildren()
    {
        var sut = new EntryNode(EntryModel.Create("A/"))
            .AddChild(new EntryNode(EntryModel.Create("A/file1.txt")))
            .AddChild(new EntryNode(EntryModel.Create("A/file2.txt")))
            .AddChild(
                new EntryNode(EntryModel.Create("A/B/"))
                    .AddChild(new EntryNode(EntryModel.Create("A/B/file1.txt")))
                    .AddChild(new EntryNode(EntryModel.Create("A/B/C/")))
            );

        var expected = ImmutableList.Create(
            "A/",
            "A/file1.txt",
            "A/file2.txt"
        );

        var actual = sut.Aggregate(
            ImmutableList<string>.Empty,
            entry => entry.Type == EntryType.File || entry.GetDirectoryName() == "A",
            (acc, entry) => acc.Add(entry.RelativePath)
        );

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CreateNodes_ForFileEntry_ReturnsSingleNode()
    {
        var entry = EntryModel.Create("file.txt");
        EntryNode[] expected = [new(entry)];

        var actual = EntryNode.CreateNodes([entry]);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CreateNodes_ForDirectoryEntry_ReturnsSingleNode()
    {
        var entry = EntryModel.Create("A/");
        EntryNode[] expected = [new(entry)];

        var actual = EntryNode.CreateNodes([entry]);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CreateNodes_ForSingleRootDirectory_WithFiles_ReturnsSingleNode()
    {
        var dir = EntryModel.Create("A/");
        var file1 = EntryModel.Create("A/file1.txt");
        var file2 = EntryModel.Create("A/file2.txt");

        EntryNode[] expected =
        [
            new EntryNode(dir)
                .AddChild(new EntryNode(file1))
                .AddChild(new EntryNode(file2)),
        ];

        var actual = EntryNode.CreateNodes([dir, file1, file2]);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CreateNodes_ForSingleRootDirectory_WithMixedChildren_ReturnsSingleNode()
    {
        var dir1 = EntryModel.Create("A/");
        var dir2 = EntryModel.Create("A/B/");

        var file1 = EntryModel.Create("A/file1.txt");
        var file2 = EntryModel.Create("A/file2.txt");
        var file3 = EntryModel.Create("A/B/file3.txt");

        EntryNode[] expected =
        [
            new EntryNode(dir1)
                .AddChild(new EntryNode(file1))
                .AddChild(new EntryNode(file2))
                .AddChild(new EntryNode(dir2).AddChild(new EntryNode(file3))),
        ];

        var actual = EntryNode.CreateNodes([dir1, file1, file2, dir2, file3]);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CreateNodes_HandlesChildrenOutOfOrder()
    {
        var dir = EntryModel.Create("A/");
        var file1 = EntryModel.Create("A/file1.txt");
        var file2 = EntryModel.Create("A/file2.txt");

        EntryNode[] expected =
        [
            new EntryNode(dir)
                .AddChild(new EntryNode(file1))
                .AddChild(new EntryNode(file2)),
        ];

        var actual = EntryNode.CreateNodes([file2, file1, dir]);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CreateNodes_AggregatesTopLevelDirectories()
    {
        var dir1 = EntryModel.Create("A/");
        var dir2 = EntryModel.Create("B/");

        var file1 = EntryModel.Create("A/file1.txt");
        var file2 = EntryModel.Create("B/file2.txt");

        EntryNode[] expected =
        [
            new EntryNode(dir1)
                .AddChild(new EntryNode(file1)),
            new EntryNode(dir2)
                .AddChild(new EntryNode(file2)),
        ];

        var actual = EntryNode.CreateNodes([dir1, file1, dir2, file2]);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void CreateNodes_OrphanedChild_IsPromotedToTopLevel()
    {
        var orphan = EntryModel.Create("B/file.txt"); // No "B/" entry

        var actual = EntryNode.CreateNodes([orphan]);

        EntryNode[] expected = [new(orphan)];
        Assert.That(actual, Is.EqualTo(expected));
    }
}
