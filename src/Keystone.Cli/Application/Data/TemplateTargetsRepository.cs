using System.Collections.Immutable;
using Keystone.Cli.Domain;
using Microsoft.Extensions.Configuration;


namespace Keystone.Cli.Application.Data;

/// <summary>
/// The template targets repository implementation based on the configuration.
/// </summary>
public class TemplateTargetsRepository(IConfiguration configuration)
    : ITemplateTargetsRepository
{
    private readonly ImmutableDictionary<string, TemplateTargetModel> _templateTargets = configuration
        .GetSection("Templates")
        .GetChildren()
        .Where(child => child.Value is not null)
        .Select(child => new TemplateTargetModel(Name: child.Key, RepositoryUrl: new Uri(child.Value!)))
        .ToImmutableDictionary(model => model.Name, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public IEnumerable<TemplateTargetModel> GetTemplateTargets()
        => _templateTargets.Values.OrderBy(template => template.Name);

    /// <inheritdoc />
    public TemplateTargetModel? GetTemplateTarget(string name)
        => _templateTargets.GetValueOrDefault(name);
}
