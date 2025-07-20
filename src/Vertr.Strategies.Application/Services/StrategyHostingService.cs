using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common;
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
        var consumer = _serviceProvider.GetRequiredService<IDataConsumer<Candle>>();
        await consumer.Consume(HandleMarketData, stoppingToken);
    }

    private async Task HandleMarketData(Candle candle, CancellationToken cancellationToken)
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.InstrumentId != candle.instrumentId)
            {
                continue;
            }

            await strategy.HandleMarketData(candle, cancellationToken);
        }
    }

    private List<StrategyBase> InitStrategies()
    {
        var repo = _serviceProvider.GetRequiredService<IStrategyMetadataRepository>();

        var res = new List<StrategyBase>();

        var strategiesMeta = repo.GetAll();

        foreach (var metadata in strategiesMeta)
        {
            var strategy = StrategyFactory.Create(metadata, _serviceProvider);
            res.Add(strategy);
        }

        return res;
    }
}
