namespace Keystone.Cli.Domain;

/// <summary>
/// The template target model.
/// </summary>
/// <param name="Name">The name associated with the template target.</param>
/// <param name="RepositoryUrl">The repository URL.</param>
public record TemplateTargetModel(string Name, Uri RepositoryUrl);
