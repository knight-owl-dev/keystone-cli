using System.IO.Compression;


namespace Keystone.Cli.Application.GitHub;

/// <summary>
/// A delegate that represents a method to extract a <see cref="ZipArchiveEntry"/> to a specified file path.
/// </summary>
/// <remarks>
/// This delegate matches the signature of <see cref="ZipFileExtensions.ExtractToFile(ZipArchiveEntry, string, bool)"/>
/// and serves as a testable seam.
/// </remarks>
public delegate void ExtractToFileDelegate(ZipArchiveEntry entry, string path, bool overwrite);
