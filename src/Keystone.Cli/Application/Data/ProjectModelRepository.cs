using System.Collections.Immutable;
using Keystone.Cli.Application.Project;
using Keystone.Cli.Application.Utility;
using Keystone.Cli.Domain.Project;


namespace Keystone.Cli.Application.Data;

/// <summary>
/// The default implementation of <see cref="IProjectModelRepository"/> that coordinates multiple
/// <see cref="IProjectModelStore"/> instances to load and save the complete <see cref="ProjectModel"/>.
/// </summary>
public class ProjectModelRepository(IEnumerable<IProjectModelStore> projectModelStores, IProjectModelPolicyEnforcer projectModelPolicyEnforcer)
    : IProjectModelRepository
{
    /// <inheritdoc />
    public Task<ProjectModel> LoadAsync(string projectPath, CancellationToken cancellationToken = default)
        => projectModelStores.AggregateAsync(
            new ProjectModel(projectPath),
            (acc, store, token) => store.LoadAsync(acc, token),
            acc => acc.With(projectModelPolicyEnforcer.ThrowIfNotLoaded),
            cancellationToken
        );

    /// <inheritdoc />
    public async Task SaveAsync(ProjectModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        var storesNeedingUpdate = projectModelStores.Aggregate(
            (
                Current: model,
                Stored: await LoadAsync(model.ProjectPath, cancellationToken).ConfigureAwait(false),
                StoresNeedingUpdate: ImmutableList.CreateBuilder<IProjectModelStore>()
            ),
            (acc, store) =>
            {
                if (store.GetContentHash(acc.Current) != store.GetContentHash(acc.Stored))
                {
                    acc.StoresNeedingUpdate.Add(store);
                }

                return acc;
            },
            acc => acc.StoresNeedingUpdate.ToImmutable()
        );

        cancellationToken.ThrowIfCancellationRequested();

        var saveTasks = storesNeedingUpdate.Select(store => store.SaveAsync(model, cancellationToken));
        await Task.WhenAll(saveTasks).ConfigureAwait(false);
    }
}
