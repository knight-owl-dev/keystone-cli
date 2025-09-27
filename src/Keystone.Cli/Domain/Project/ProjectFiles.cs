namespace Keystone.Cli.Domain.Project;

/// <summary>
/// Defines well-known filenames used throughout a Keystone project.
/// </summary>
public static class ProjectFiles
{
    /// <summary>
    /// The <c>.env</c> file.
    /// </summary>
    /// <remarks>
    /// This file defines the basic metadata for a book project. Keystone uses it
    /// to populate document headers, filenames, and publishing settings before building
    /// books inside a Docker container.
    /// </remarks>
    public const string EnvFileName = ".env";

    /// <summary>
    /// The <c>publish.txt</c> file.
    /// </summary>
    /// <remarks>
    /// This file defines the order in which Markdown files are compiled into your book.
    /// </remarks>
    public const string PublishFileName = "publish.txt";

    /// <summary>
    /// The <c>pandoc.yaml</c> file.
    /// </summary>
    /// <remarks>
    /// Keystone-generated Pandoc metadata file.
    /// </remarks>
    public const string PandocFileName = "pandoc.yaml";

    /// <summary>
    /// The <c>.keystone/sync.json</c> file.
    /// </summary>
    /// <remarks>
    /// This file tracks the state of the Keystone core files in your project.
    /// </remarks>
    public const string KeystoneSyncFileName = ".keystone/sync.json";
}
