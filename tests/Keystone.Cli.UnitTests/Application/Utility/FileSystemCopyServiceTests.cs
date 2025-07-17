using Keystone.Cli.Application.FileSystem;
using Keystone.Cli.Domain.FileSystem;
using Keystone.Cli.UnitTests.Domain.FileSystem;
using Keystone.Cli.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Application.Utility;

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
    public void Copy_EntryProvider_IsNull_ThrowsArgumentNullException()
    {
        var sut = Ctor();

        Assert.That(
            () => sut.Copy(entryProvider: null!, destinationPath: ".", overwrite: false),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("entryProvider")
        );
    }

    [Test]
    public void Copy_DestinationPath_IsNull_ThrowsArgumentNullException()
    {
        var entryProvider = Substitute.For<IEntryProvider>();
        var sut = Ctor();

        Assert.That(
            () => sut.Copy(entryProvider, destinationPath: null!, overwrite: false),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("destinationPath")
        );
    }

    [Test]
    public void Copy_DestinationPath_IsEmpty_ThrowsArgumentException()
    {
        var entryProvider = Substitute.For<IEntryProvider>();
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

        var entryProvider = EntryProvider.Fake([directoryEntry]);

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

        var entryProvider = EntryProvider.Fake([directoryEntry]);

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

        var entryProvider = EntryProvider.Fake([fileEntry]);

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

        var entryProvider = EntryProvider.Fake([fileEntry]);

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

        var entryProvider = EntryProvider.Fake([fileEntry]);

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

        var entryProvider = EntryProvider.Fake([directoryEntry1, directoryEntry2]);

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

        var entryProvider = EntryProvider.Fake([fileEntry1, fileEntry2]);

        var predicate = Substitute.For<Func<EntryModel, bool>>();
        predicate.Invoke(fileEntry1).Returns(false);
        predicate.Invoke(fileEntry2).Returns(true);

        var sut = Ctor(fileSystemService: fileSystemService);

        sut.Copy(entryProvider, destinationPath, overwrite: false, predicate);

        entryProvider.DidNotReceive().ExtractToFile(fileEntry1, fileFullPath1);
        entryProvider.Received(1).ExtractToFile(fileEntry2, fileFullPath2);
    }
}
