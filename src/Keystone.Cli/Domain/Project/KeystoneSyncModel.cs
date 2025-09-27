namespace Keystone.Cli.Domain.Project;

/// <summary>
/// Represents the model for synchronizing Keystone project metadata.
/// </summary>
public record KeystoneSyncModel(string TemplateRepositoryName)
{
    /// <summary>
    /// The source repository name the Keystone template was synced from, if applicable.
    /// </summary>
    /// <example>
    /// keystone
    /// </example>
    public string? SourceRepositoryName { get; init; }

    /// <summary>
    /// The branch in the source repository the Keystone template was synced from, if applicable.
    /// </summary>
    /// <example>
    /// slim
    /// </example>
    public string? SourceRepositoryBranch { get; init; }

    /// <summary>
    /// The commit hash in the source repository the Keystone template was synced from, if applicable.
    /// </summary>
    /// <example>
    /// 6589f5b0f0cd98689946f0398f81aa737d657741
    /// </example>
    public string? SourceRepositoryCommit { get; init; }

    /// <summary>
    /// The name of the template repository the project is currently based on.
    /// </summary>
    /// <example>
    /// keystone-template-core-slim
    /// </example>
    public string TemplateRepositoryName { get; init; } = TemplateRepositoryName;

    /// <summary>
    /// The timestamp of the last synchronization with the Keystone template repository.
    /// </summary>
    public DateTime? TemplateRepositorySyncedAt { get; init; }

    /// <summary>
    /// The note associated with the last synchronization, if any.
    /// </summary>
    public string? Note { get; init; }
}
