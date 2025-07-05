namespace Keystone.Cli.Domain.FileSystem;

/// <summary>
/// The file system entry model.
/// </summary>
/// <param name="Type">The type of entry.</param>
/// <param name="Name">The name of the entry.</param>
/// <param name="RelativePath">Relative path for this entry.</param>
public record EntryModel(EntryType Type, string Name, string RelativePath)
{
    public string GetFullPath(string rootPath)
        => Path.Combine(rootPath, this.RelativePath);
}
