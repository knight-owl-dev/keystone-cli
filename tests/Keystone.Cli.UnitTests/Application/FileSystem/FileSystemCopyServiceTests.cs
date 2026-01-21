using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.UnitTests.Domain.FileSystem;
using Keystone.Cli.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.FileSystem;

[TestFixture, Parallelizable(ParallelScope.All)]
public class FileSystemCopyServiceTests
{
    private static FileSystemCopyService Ctor(
        IFileSystemService? fileSystemService = null,
        ILogger<FileSystemCopyService>? logger = null
    )
        => new(
            fileSystemService ?? Substitute.For<IFileSystemService>(),
            logger ?? new TestLogger<FileSystemCopyService>()
        );

    [Test]
    public void Copy_EntryCollection_IsNull_ThrowsArgumentNullException()
    {
        var sut = Ctor();

        Assert.That(
            () => sut.Copy(entryCollection: null!, destinationPath: ".", overwrite: false),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("entryCollection")
        );
    }

    [Test]
    public void Copy_DestinationPath_IsNull_ThrowsArgumentNullException()
    {
        var entryProvider = Substitute.For<IEntryCollection>();
        var sut = Ctor();

        Assert.That(
            () => sut.Copy(entryProvider, destinationPath: null!, overwrite: false),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("destinationPath")
        );
    }

    [Test]
    public void Copy_DestinationPath_IsEmpty_ThrowsArgumentException()
    {
        var entryProvider = Substitute.For<IEntryCollection>();
        var sut = Ctor();

        Assert.That(
            () => sut.Copy(entryProvider, destinationPath: string.Empty, overwrite: false),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("destinationPath")
        );
    }

    [Test]
    public void Copy_SingleDirectory_DoesNotExist_CreatesDirectory()
    {
        const string destinationPath = "test/path";

        var directoryEntry = EntryModel.Create("A/");
        var directoryFullPath = directoryEntry.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.DirectoryExists(directoryFullPath).Returns(false);

        var entryProvider = EntryCollection.Fake([directoryEntry]);

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false);

        fileSystemService.Received(1).CreateDirectory(directoryFullPath);
    }

    [Test]
    public void Copy_SingleDirectory_AlreadyExists_DoesNotCreateDirectory()
    {
        const string destinationPath = "test/path";

        var directoryEntry = EntryModel.Create("A/");
        var directoryFullPath = directoryEntry.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.DirectoryExists(directoryFullPath).Returns(true);

        var entryProvider = EntryCollection.Fake([directoryEntry]);

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false);

        fileSystemService.DidNotReceive().CreateDirectory(directoryFullPath);
    }

    [Test]
    public void Copy_SingleFile_DoesNotExist_ExtractsToFile()
    {
        const string destinationPath = "test/path";

        var fileEntry = EntryModel.Create("file.txt");
        var fileFullPath = fileEntry.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.FileExists(fileFullPath).Returns(false);

        var entryProvider = EntryCollection.Fake([fileEntry]);

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false);

        entryProvider.Received(1).ExtractToFile(fileEntry, fileFullPath);
    }

    [Test]
    public void Copy_SingleFile_AlreadyExist_CanOverwrite_ExtractsToFile()
    {
        const string destinationPath = "test/path";

        var fileEntry = EntryModel.Create("file.txt");
        var fileFullPath = fileEntry.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.FileExists(fileFullPath).Returns(true);

        var entryProvider = EntryCollection.Fake([fileEntry]);

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: true);

        entryProvider.Received(1).ExtractToFile(fileEntry, fileFullPath);
    }

    [Test]
    public void Copy_SingleFile_AlreadyExist_OverwritingIsForbidden_DoesNotExtractToFile()
    {
        const string destinationPath = "test/path";

        var fileEntry = EntryModel.Create("file.txt");
        var fileFullPath = fileEntry.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.FileExists(fileFullPath).Returns(true);

        var entryProvider = EntryCollection.Fake([fileEntry]);

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false);

        entryProvider.DidNotReceive().ExtractToFile(fileEntry, fileFullPath);
    }

    [Test]
    public void Copy_UsesPredicate_ToSkipDirectories()
    {
        const string destinationPath = "test/path";

        var directoryEntry1 = EntryModel.Create("A/");
        var directoryFullPath1 = directoryEntry1.GetFullPath(destinationPath);

        var directoryEntry2 = EntryModel.Create("B/");
        var directoryFullPath2 = directoryEntry2.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.DirectoryExists(directoryFullPath1).Returns(false);
        fileSystemService.DirectoryExists(directoryFullPath2).Returns(false);

        var entryProvider = EntryCollection.Fake([directoryEntry1, directoryEntry2]);

        var predicate = Substitute.For<Func<EntryModel, bool>>();
        predicate.Invoke(directoryEntry1).Returns(false);
        predicate.Invoke(directoryEntry2).Returns(true);

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false, predicate);

        fileSystemService.DidNotReceive().CreateDirectory(directoryFullPath1);
        fileSystemService.Received(1).CreateDirectory(directoryFullPath2);
    }

    [Test]
    public void Copy_UsesPredicate_ToSkipFiles()
    {
        const string destinationPath = "test/path";

        var fileEntry1 = EntryModel.Create("file1.txt");
        var fileFullPath1 = fileEntry1.GetFullPath(destinationPath);

        var fileEntry2 = EntryModel.Create("file2.txt");
        var fileFullPath2 = fileEntry2.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.FileExists(fileFullPath1).Returns(false);
        fileSystemService.FileExists(fileFullPath2).Returns(false);

        var entryProvider = EntryCollection.Fake([fileEntry1, fileEntry2]);

        var predicate = Substitute.For<Func<EntryModel, bool>>();
        predicate.Invoke(fileEntry1).Returns(false);
        predicate.Invoke(fileEntry2).Returns(true);

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false, predicate);

        entryProvider.DidNotReceive().ExtractToFile(fileEntry1, fileFullPath1);
        entryProvider.Received(1).ExtractToFile(fileEntry2, fileFullPath2);
    }

    [Test]
    public void Copy_PredicateSkipsDirectory_IgnoresSubdirectoryInSkippedDirectory()
    {
        const string destinationPath = "test/path";

        var directoryEntry = EntryModel.Create("A/");
        var subDirectoryEntry = EntryModel.Create("A/subdirectory/");
        var directoryFullPath = directoryEntry.GetFullPath(destinationPath);
        var subDirectoryFullPath = subDirectoryEntry.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.DirectoryExists(directoryFullPath).Returns(false);
        fileSystemService.DirectoryExists(subDirectoryFullPath).Returns(false);

        var entryProvider = EntryCollection.Fake([directoryEntry, subDirectoryEntry]);

        var predicate = Substitute.For<Func<EntryModel, bool>>();
        predicate.Invoke(directoryEntry).Returns(false); // Skip the main directory

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false, predicate);

        fileSystemService.DidNotReceive().CreateDirectory(directoryFullPath);
        fileSystemService.DidNotReceive().CreateDirectory(subDirectoryFullPath);
        predicate.DidNotReceive().Invoke(subDirectoryEntry);
    }

    [Test]
    public void Copy_PredicateSkipsDirectory_IgnoresFilesInSkippedDirectory()
    {
        const string destinationPath = "test/path";

        var directoryEntry = EntryModel.Create("A/");
        var fileEntry = EntryModel.Create("A/file.txt");
        var directoryFullPath = directoryEntry.GetFullPath(destinationPath);
        var fileFullPath = fileEntry.GetFullPath(destinationPath);

        var fileSystemService = Substitute.For<IFileSystemService>();
        fileSystemService.DirectoryExists(destinationPath).Returns(true);
        fileSystemService.DirectoryExists(directoryFullPath).Returns(false);
        fileSystemService.FileExists(fileFullPath).Returns(false);

        var entryProvider = EntryCollection.Fake([directoryEntry, fileEntry]);

        var predicate = Substitute.For<Func<EntryModel, bool>>();
        predicate.Invoke(directoryEntry).Returns(false); // Skip the main directory

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false, predicate);

        fileSystemService.DidNotReceive().CreateDirectory(directoryFullPath);
        predicate.DidNotReceive().Invoke(fileEntry);
        entryProvider.DidNotReceive().ExtractToFile(fileEntry, fileFullPath);
    }
}
