using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Strategies.Application.StrategiesImpl;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.Services;

internal class StrategyHostingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private List<StrategyBase> _strategies;

    public StrategyHostingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _strategies = InitStrategies();
        var consumer = _serviceProvider.GetRequiredService<IMarketDataConsumer>();
        await consumer.Consume(HandleMarketData, stoppingToken);
    }

    private Task HandleMarketData(Candle candle, CancellationToken cancellationToken)
    {
        Parallel.ForEach(_strategies, s => s.HandleMarketData(candle, cancellationToken));
        return Task.CompletedTask;
    }

    private List<StrategyBase> InitStrategies()
    {
        var repo = _serviceProvider.GetRequiredService<IStrategyMetadataRepository>();

        var res = new List<StrategyBase>();

        var strategiesMeta = repo.GetAll();

        foreach (var metadata in strategiesMeta)
        {
            var strategy = StrategyFactory.Create(metadata);
            res.Add(strategy);
        }

        return res;
    }
}
