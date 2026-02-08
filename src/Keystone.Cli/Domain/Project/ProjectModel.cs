using System.Diagnostics.CodeAnalysis;


namespace Keystone.Cli.Domain.Project;

/// <summary>
/// The project model with basic properties.
/// </summary>
public record ProjectModel(string ProjectPath)
{
    /// <summary>
    /// The root path of your Keystone project.
    /// </summary>
    public string ProjectPath { get; init; } = ProjectPath;

    /// <summary>
    /// The name of your project.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is used to name generated artifacts (e.g., keystone.pdf, .epub).
    /// It should be a simple, lowercase string without spaces or special characters.
    /// </para>
    /// <para>
    /// Note: This is not the title of your book, but the name of the project.
    /// </para>
    /// <para>
    /// Sourced from the <c>KEYSTONE_PROJECT</c> key in <c>project.conf</c> file.
    /// </para>
    /// </remarks>
    /// <example>
    /// hello-world
    /// </example>
    public string? ProjectName { get; init; }

    /// <summary>
    /// The cover image for your book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the path to the cover image file in your <c>./assets</c> folder.
    /// It should be a valid image format (e.g., PNG, JPG).
    /// </para>
    /// <para>
    /// Sourced from the <c>cover-image</c> property in <c>pandoc.yaml</c>.
    /// Used only when building EPUB output.
    /// </para>
    /// </remarks>
    /// <example>
    /// ./assets/cover.png
    /// </example>
    public string? CoverImage { get; init; }

    /// <summary>
    /// Paper size for LaTeX (PDF only).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Corresponds to LaTeX's <c>papersize</c> option (no "paper" suffix).
    /// Leave empty to use the LaTeX default (usually letter or a4).
    /// </para>
    /// <para>
    /// Sourced from the <c>papersize</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// a4
    /// </example>
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public string? LatexPapersize { get; init; }

    /// <summary>
    /// Page geometry (margins) for LaTeX.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use LaTeX geometry syntax: margin=1in or top=20mm, bottom=25mm, etc.
    /// Leave empty to use the LaTeX default (margin=1in).
    /// </para>
    /// <para>
    /// Sourced from the <c>geometry</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// margin=1in
    /// </example>
    public string? LatexGeometry { get; init; }

    /// <summary>
    /// Base font size for LaTeX document.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Accepted values: 10pt, 11pt, 12pt. Leave empty to use the LaTeX default (10pt).
    /// </para>
    /// <para>
    /// Sourced from the <c>fontsize</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// 12pt
    /// </example>
    public string? LatexFontsize { get; init; }

    /// <summary>
    /// Font family for the main body text (requires XeLaTeX engine).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Passed as <c>-V fontfamily=...</c>, mapped to \setmainfont under the hood.
    /// Leave empty to use the default LaTeX font (Computer Modern).
    /// </para>
    /// <para>
    /// Sourced from the <c>fontfamily</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// libertine
    /// </example>
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public string? LatexFontfamily { get; init; }

    /// <summary>
    /// Docker Compose project name (used to name networks, containers, and volumes).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Defines the Compose project namespace. By default, it's prefixed with "keystone-" for clarity and conflict avoidance.
    /// </para>
    /// <para>
    /// Sourced from the <c>KEYSTONE_DOCKER_COMPOSE_PROJECT</c> key in <c>project.conf</c> file.
    /// </para>
    /// </remarks>
    /// <example>
    /// keystone-${KEYSTONE_PROJECT}
    /// </example>
    public string? DockerComposeProject { get; init; }

    /// <summary>
    /// Docker image name for your project.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Used when building the Docker image. By default, matches the Compose project name, but can be overridden.
    /// </para>
    /// <para>
    /// Sourced from the <c>KEYSTONE_DOCKER_IMAGE</c> key in <c>project.conf</c> file.
    /// </para>
    /// </remarks>
    /// <example>
    /// ${KEYSTONE_DOCKER_COMPOSE_PROJECT}
    /// </example>
    public string? DockerImage { get; init; }

    /// <summary>
    /// The title of your book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the main title of your book, which will appear on the cover and in metadata.
    /// </para>
    /// <para>
    /// Sourced from the <c>title</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// Keystone
    /// </example>
    public string? Title { get; init; }

    /// <summary>
    /// The subtitle of your book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Optional subtitle for additional context; appears on cover and in metadata.
    /// </para>
    /// <para>
    /// Sourced from the <c>subtitle</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// The sample book using the Keystone project
    /// </example>
    public string? Subtitle { get; init; }

    /// <summary>
    /// The author of your book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Name of the author or organization; appears on cover and in metadata.
    /// </para>
    /// <para>
    /// Sourced from the <c>author</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// Knight Owl LLC
    /// </example>
    public string? Author { get; init; }

    /// <summary>
    /// The date of your book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Publication date; can be a specific date, "auto", or "disabled".
    /// </para>
    /// <para>
    /// Sourced from the <c>date</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// auto
    /// </example>
    public string? Date { get; init; }

    /// <summary>
    /// The language (culture) of your book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sets document language, affecting metadata and language-specific behavior.
    /// </para>
    /// <para>
    /// Sourced from the <c>lang</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// en-US
    /// </example>
    public string? Lang { get; init; }

    /// <summary>
    /// The footer copyright text.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Copyright text for the footer; can be "auto", "disabled", or custom text.
    /// </para>
    /// <para>
    /// Sourced from the <c>footer-copyright</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// auto
    /// </example>
    public string? FooterCopyright { get; init; }

    /// <summary>
    /// A short description of your book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Brief summary included in metadata.
    /// </para>
    /// <para>
    /// Sourced from the <c>description</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// This is a tribute to beginnings. In every language, in every framework, "Hello, world" is the first spark — the moment code speaks back. This sample book is your invitation to begin — to build something real, reproducible, and yours.
    /// </example>
    public string? Description { get; init; }

    /// <summary>
    /// A list of keywords for your book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Keywords/tags for metadata, indexing, or cataloging.
    /// </para>
    /// <para>
    /// Sourced from the <c>keywords</c> property in <c>pandoc.yaml</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// [ "hello-world", "keystone", "book" ]
    /// </example>
    public IReadOnlyList<string>? Keywords { get; init; }

    /// <summary>
    /// The Keystone sync model for project metadata synchronization.
    /// </summary>
    public KeystoneSyncModel? KeystoneSync { get; init; }

    /// <summary>
    /// The list of Markdown content files to include in the published book.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property defines the order in which Markdown files are compiled into your book.
    /// Include appendices, prefaces, and other sections in the order you want them to appear — one per line.
    /// The first file listed will be the first chapter, etc.
    /// </para>
    /// <para>
    /// Paths are relative to the root of your book project. You can also include files from subdirectories.
    /// Keystone recognizes only <c>appendix/</c>, <c>assets/</c>, and <c>chapters/</c> for publishing:
    /// <list type="bullet">
    /// <item>DO include contents of <c>appendix/</c> and <c>chapters/</c> for publishing.</item>
    /// <item>DO NOT include any files from other directories.</item>
    /// <item>Contents of <c>assets/</c> are automatically included if referenced inside your Markdown files.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Sourced from the <c>publish.txt</c> file in the project root.
    /// </para>
    /// </remarks>
    /// <example>
    /// [ "chapters/introduction.md", "chapters/chapter-1.md", "appendix/appendix-a.md" ]
    /// </example>
    public IReadOnlyList<string>? ContentFilePaths { get; init; }
}
