using Keystone.Cli.Application.Data;
using Keystone.Cli.Domain;
using Microsoft.Extensions.Configuration;


namespace Keystone.Cli.Application;

/// <summary>
/// The Keystone template service implementation.
/// </summary>
public class TemplateService(
    IConfiguration configuration,
    ITemplateTargetsRepository templateTargetsRepository
)
    : ITemplateService
{
    /// <summary>
    /// The key for the default template name in the configuration.
    /// </summary>
    private const string DefaultTemplateKey = "DefaultTemplate";

    /// <summary>
    /// The default template name as specified in the configuration.
    /// </summary>
    private string DefaultTemplateName { get; } = configuration.GetSection(DefaultTemplateKey).Value
        ?? throw new InvalidOperationException($"The '{DefaultTemplateKey}' key is not set in the configuration.");

    /// <inheritdoc />
    public TemplateTargetModel GetTemplateTarget(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            return templateTargetsRepository.GetTemplateTarget(name)
                ?? throw CreateKeyNotFoundException(name);
        }

        return templateTargetsRepository.GetTemplateTarget(this.DefaultTemplateName)
            ?? throw CreateKeyNotFoundException(this.DefaultTemplateName);
    }

    /// <inheritdoc />
    public IEnumerable<TemplateTargetModel> GetTemplateTargets()
        => templateTargetsRepository.GetTemplateTargets();

    private KeyNotFoundException CreateKeyNotFoundException(string name)
    {
        var supportedTargets = string.Join(", ", templateTargetsRepository.GetTemplateTargets().Select(model => model.Name));

        return new KeyNotFoundException($"Template target '{name}' not found. Supported targets: {supportedTargets}.");
    }
}
