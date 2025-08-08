using Microsoft.Extensions.DependencyInjection;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application;

internal class StrategyRepository : IStrategyRepository
{
    private readonly IServiceProvider _serviceProvider;

    private Dictionary<Guid, IStrategy> _strategies = [];

    public StrategyRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        _strategies = await InitStrategies();
    }

    public Task<IStrategy[]> GetActiveStrategies()
        => Task.FromResult(_strategies.Values.ToArray());

    public Task Update(StrategyMetadata strategyMetadata)
    {
        // TODO: Use ConcurrentDictionary
        return Task.CompletedTask;
    }

    private async Task<Dictionary<Guid, IStrategy>> InitStrategies()
    {
        var res = new Dictionary<Guid, IStrategy>();
        var repository = _serviceProvider.GetRequiredService<IStrategyMetadataRepository>();
        var strategiesMeta = await repository.GetAll();

        foreach (var metadata in strategiesMeta)
        {
            if (!metadata.IsActive)
            {
                continue;
            }

            var strategy = StrategyFactory.Create(metadata, _serviceProvider);
            res[strategy.Id] = strategy;
        }

        return res;
    }
}
