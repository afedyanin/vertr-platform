using Microsoft.Extensions.DependencyInjection;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application;

internal class StrategyRepository : IStrategyRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IStrategyMetadataRepository _metadataRepository;
    private readonly IStrategyFactory _strategyFactory;

    private Dictionary<Guid, IStrategy> _strategies = [];

    public StrategyRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _metadataRepository = _serviceProvider.GetRequiredService<IStrategyMetadataRepository>();
        _strategyFactory = _serviceProvider.GetRequiredService<IStrategyFactory>();
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _strategies = await InitStrategies(cancellationToken);
    }

    public Task<IStrategy[]> GetActiveStrategies()
        => Task.FromResult(_strategies.Values.ToArray());


    public async Task Update(StrategyMetadata strategyMetadata, CancellationToken cancellationToken = default)
    {
        await Delete(strategyMetadata.Id, cancellationToken);
        await Add(strategyMetadata, cancellationToken);
    }

    public async Task Delete(Guid strategyId, CancellationToken cancellationToken = default)
    {
        if (!_strategies.TryGetValue(strategyId, out var existingStrategy))
        {
            return;
        }

        _strategies.Remove(strategyId);
        await existingStrategy.OnStop(cancellationToken);
        existingStrategy.Dispose();
    }

    private async Task Add(StrategyMetadata strategyMetadata, CancellationToken cancellationToken = default)
    {
        if (!strategyMetadata.IsActive)
        {
            return;
        }

        var strategy = _strategyFactory.Create(strategyMetadata, _serviceProvider);
        _strategies[strategy.Id] = strategy;
        await strategy.OnStart(cancellationToken);
    }

    private async Task<Dictionary<Guid, IStrategy>> InitStrategies(CancellationToken cancellationToken = default)
    {
        var strategiesMeta = await _metadataRepository.GetAll();

        var res = new Dictionary<Guid, IStrategy>();

        foreach (var metadata in strategiesMeta)
        {
            await Add(metadata, cancellationToken);
        }

        return res;
    }
}
