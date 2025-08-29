using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application;

internal class StrategyRepository : IStrategyRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IStrategyMetadataRepository _metadataRepository;
    private readonly IStrategyFactory _strategyFactory;

    private Dictionary<Guid, IStrategy> _strategies = [];

    private readonly ILogger<StrategyRepository> _logger;

    public StrategyRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _metadataRepository = _serviceProvider.GetRequiredService<IStrategyMetadataRepository>();
        _strategyFactory = _serviceProvider.GetRequiredService<IStrategyFactory>();
        _logger = _serviceProvider.GetRequiredService<ILogger<StrategyRepository>>();
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await InitStrategies(cancellationToken);
        _logger.LogInformation($"{_strategies.Count} strategies started.");
    }

    public Task<IStrategy[]> GetActiveStrategies()
        => Task.FromResult(_strategies.Values.ToArray());

    public async Task Update(StrategyMetadata strategyMetadata, CancellationToken cancellationToken = default)
    {
        await Delete(strategyMetadata.Id, cancellationToken);
        await Add(strategyMetadata, cancellationToken);
    }

    private async Task InitStrategies(CancellationToken cancellationToken = default)
    {
        var strategiesMeta = await _metadataRepository.GetAll();

        foreach (var metadata in strategiesMeta)
        {
            await Add(metadata, cancellationToken);
        }
    }

    private async Task Add(StrategyMetadata strategyMetadata, CancellationToken cancellationToken = default)
    {
        if (!strategyMetadata.IsActive)
        {
            _logger.LogDebug($"Skipping inactive strategy Id={strategyMetadata.Id} {strategyMetadata.Name}");
            return;
        }

        var strategy = _strategyFactory.Create(strategyMetadata, _serviceProvider);
        _strategies[strategy.Id] = strategy;

        _logger.LogDebug($"Starting strategy Id={strategyMetadata.Id} {strategyMetadata.Name}");
        await strategy.OnStart(backtest: null, cancellationToken);
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
}
