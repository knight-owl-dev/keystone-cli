using System.Collections.Immutable;
using Keystone.Cli.Domain.FileSystem;


namespace Keystone.Cli.UnitTests.Domain.FileSystem.TestData;

/// <summary>
/// Provides standard Keystone project layouts for unit testing.
/// </summary>
public static class StandardProjectLayout
{
    /// <summary>
    /// Represents a minimal directory structure for a Keystone project template.
    /// </summary>
    public static readonly ImmutableArray<EntryModel> MinimalTemplateEntries =
    [
        EntryModel.Create(".docker/"),
        EntryModel.Create(".docker/docker-compose.yaml"),
        EntryModel.Create(".dockerignore"),
        EntryModel.Create(".editorconfig"),
        EntryModel.Create(".env"),
        EntryModel.Create(".gitattributes"),
        EntryModel.Create(".gitignore"),
        EntryModel.Create(".keystone/"),
        EntryModel.Create(".keystone/sync.json"),
        EntryModel.Create(".licenses/"),
        EntryModel.Create(".licenses/Keystone.md"),
        EntryModel.Create(".licenses/Pandoc.md"),
        EntryModel.Create("appendix/"),
        EntryModel.Create("appendix/.gitkeep"),
        EntryModel.Create("artifacts/"),
        EntryModel.Create("artifacts/.gitkeep"),
        EntryModel.Create("assets/"),
        EntryModel.Create("assets/.gitkeep"),
        EntryModel.Create("chapters/"),
        EntryModel.Create("chapters/.gitkeep"),
        EntryModel.Create("drafts/"),
        EntryModel.Create("drafts/.gitkeep"),
        EntryModel.Create("research/"),
        EntryModel.Create("research/.gitkeep"),
        EntryModel.Create("Makefile"),
        EntryModel.Create("NOTICE.md"),
        EntryModel.Create("pandoc.yaml"),
        EntryModel.Create("publish.txt"),
        EntryModel.Create("README.md"),
    ];

    /// <summary>
    /// Represents the full core directory structure for a Keystone project template.
    /// </summary>
    public static readonly ImmutableArray<EntryModel> FullTemplateEntries =
    [
        EntryModel.Create(".docker/"),
        EntryModel.Create(".docker/Dockerfile"),
        EntryModel.Create(".docker/docker-compose.yaml"),
        EntryModel.Create(".dockerignore"),
        EntryModel.Create(".editorconfig"),
        EntryModel.Create(".env"),
        EntryModel.Create(".gitattributes"),
        EntryModel.Create(".gitignore"),
        EntryModel.Create(".keystone/"),
        EntryModel.Create(".keystone/sample/"),
        EntryModel.Create(".keystone/sample/.env"),
        EntryModel.Create(".keystone/sample/appendix/"),
        EntryModel.Create(".keystone/sample/appendix/appendix-a.md"),
        EntryModel.Create(".keystone/sample/assets/"),
        EntryModel.Create(".keystone/sample/assets/keystone-cover.jpg"),
        EntryModel.Create(".keystone/sample/assets/keystone-example.jpg"),
        EntryModel.Create(".keystone/sample/chapters/"),
        EntryModel.Create(".keystone/sample/chapters/chapter-1.md"),
        EntryModel.Create(".keystone/sample/chapters/chapter-2.md"),
        EntryModel.Create(".keystone/sample/chapters/introduction.md"),
        EntryModel.Create(".keystone/sample/pandoc.yaml"),
        EntryModel.Create(".keystone/sample/publish.txt"),
        EntryModel.Create(".keystone/sync.json"),
        EntryModel.Create(".licenses/"),
        EntryModel.Create(".licenses/Keystone.md"),
        EntryModel.Create(".licenses/Pandoc.md"),
        EntryModel.Create(".pandoc/"),
        EntryModel.Create(".pandoc/filters/"),
        EntryModel.Create(".pandoc/filters/div-dialog.lua"),
        EntryModel.Create(".pandoc/filters/div-latex-only.lua"),
        EntryModel.Create(".pandoc/filters/div-pagebreak.lua"),
        EntryModel.Create(".pandoc/filters/div-poem-date.lua"),
        EntryModel.Create(".pandoc/filters/keystone.lua"),
        EntryModel.Create(".pandoc/import.sh"),
        EntryModel.Create(".pandoc/includes/"),
        EntryModel.Create(".pandoc/includes/base-style.css"),
        EntryModel.Create(".pandoc/includes/base-style.tex"),
        EntryModel.Create(".pandoc/metadata/"),
        EntryModel.Create(".pandoc/metadata/book.yaml"),
        EntryModel.Create(".pandoc/publish.sh"),
        EntryModel.Create("Makefile"),
        EntryModel.Create("NOTICE.md"),
        EntryModel.Create("README.md"),
        EntryModel.Create("appendix/"),
        EntryModel.Create("appendix/.gitkeep"),
        EntryModel.Create("artifacts/"),
        EntryModel.Create("artifacts/.gitkeep"),
        EntryModel.Create("assets/"),
        EntryModel.Create("assets/.gitkeep"),
        EntryModel.Create("chapters/"),
        EntryModel.Create("chapters/.gitkeep"),
        EntryModel.Create("drafts/"),
        EntryModel.Create("drafts/.gitkeep"),
        EntryModel.Create("pandoc.yaml"),
        EntryModel.Create("publish.txt"),
        EntryModel.Create("research/"),
        EntryModel.Create("research/.gitkeep"),
    ];

    /// <summary>
    /// Known Git-related entries.
    /// </summary>
    public static readonly ImmutableHashSet<EntryModel> GitEntries =
    [
        EntryModel.Create(".git/"),
        EntryModel.Create(".gitignore"),
        EntryModel.Create(".gitattributes"),
        EntryModel.Create(".gitkeep"),
        EntryModel.Create(".gitmodules"),
        EntryModel.Create("appendix/.gitkeep"),
        EntryModel.Create("artifacts/.gitkeep"),
        EntryModel.Create("assets/.gitkeep"),
        EntryModel.Create("chapters/.gitkeep"),
        EntryModel.Create("drafts/.gitkeep"),
        EntryModel.Create("research/.gitkeep"),
    ];
}
