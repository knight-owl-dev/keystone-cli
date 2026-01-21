using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using Keystone.Cli.Application.GitHub;
using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.UnitTests.Domain.FileSystem.TestData;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.GitHub;

[TestFixture, Parallelizable(ParallelScope.All)]
[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
public class GitHubZipEntryCollectionTests
{
    private static GitHubZipEntryCollection Ctor(ExtractToFileHandler? extractToFileHandler = null, ZipArchive? archive = null)
        => new(
            extractToFileHandler ?? Substitute.For<ExtractToFileHandler>(),
            archive ?? GitHubSourceCodeArchiveFactory.CreateEmpty()
        );

    private static readonly TestCaseData[] CtorTestCases =
    [
        new("full-template", StandardProjectLayout.FullTemplateEntries)
        {
            TestName = nameof(StandardProjectLayout.FullTemplateEntries),
        },
        new("minimal-template", StandardProjectLayout.MinimalTemplateEntries)
        {
            TestName = nameof(StandardProjectLayout.MinimalTemplateEntries),
        },
    ];

    [Test]
    public void Ctor_ForEmptyArchive()
    {
        using var actual = Ctor();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Has.Count.Zero);
            Assert.That(actual, Is.Empty);
        }
    }

    [TestCaseSource(nameof(CtorTestCases))]
    public void Ctor_ForArchive(string rootDirectoryName, ImmutableArray<EntryModel> entries)
    {
        using var actual = Ctor(archive: GitHubSourceCodeArchiveFactory.Create(rootDirectoryName, entries));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Has.Count.EqualTo(entries.Length));
            Assert.That(actual, Is.EquivalentTo(entries));
        }
    }

    [Test]
    public void Dispose_DisposesZipArchive()
    {
        var zipArchive = GitHubSourceCodeArchiveFactory.CreateEmpty();

        var sut = Ctor(archive: zipArchive);
        sut.Dispose();

        Assert.That(() => zipArchive.Entries, Throws.TypeOf<ObjectDisposedException>());
    }

    [Test, SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public void ExtractToFile_Entry_IsDirectory_ThrowsArgumentException()
    {
        const string destinationFileName = "destination.txt";
        var directory = EntryModel.Create("some-directory/");

        using var sut = Ctor(archive: GitHubSourceCodeArchiveFactory.Create(rootDirectoryName: "project", [directory]));

        Assert.That(
            () => sut.ExtractToFile(entry: directory, destinationFileName),
            Throws.ArgumentException.With.Message.Contains($"The entry type must be '{EntryType.File}'.")
        );
    }

    [Test, SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public void ExtractToFile_Entry_DoesNotExist_ThrowsInvalidOperationException()
    {
        const string destinationFileName = "destination.txt";
        var file = EntryModel.Create("some-file.txt");

        using var sut = Ctor(archive: GitHubSourceCodeArchiveFactory.CreateEmpty());

        Assert.That(
            () => sut.ExtractToFile(entry: file, destinationFileName),
            Throws.InvalidOperationException
                .With.Message.Contains($"The '{file.Name}' entry does not exist in the zip archive at {file.RelativePath}.")
        );
    }

    [Test, SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public void ExtractToFile_Entry_ExtractsFile()
    {
        const string destinationFileName = "~/abc/destination.txt";

        var noiseFileEntry = EntryModel.Create("noise-file.txt");
        var targetFileEntry = EntryModel.Create("target-file.txt");

        var extractToFileHandler = Substitute.For<ExtractToFileHandler>();

        using var sut = Ctor(
            extractToFileHandler,
            GitHubSourceCodeArchiveFactory.Create(rootDirectoryName: "project", [noiseFileEntry, targetFileEntry])
        );

        sut.ExtractToFile(entry: targetFileEntry, destinationFileName);

        extractToFileHandler.Received(1).Invoke(
            Arg.Is<ZipArchiveEntry>(zipEntry => zipEntry.Name == targetFileEntry.Name),
            destinationFileName,
            overwrite: true
        );
    }
}
