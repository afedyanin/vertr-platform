using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common;
using Vertr.Strategies.Application.StrategiesImpl;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.Services;

internal class StrategyHostingService : DataConsumerServiceBase<Candle>
{
    private List<StrategyBase> _strategies;

    public StrategyHostingService(
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _strategies = InitStrategies();
        return base.ExecuteAsync(stoppingToken);
    }

    protected override async Task Handle(Candle data, CancellationToken cancellationToken = default)
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.InstrumentId != data.instrumentId)
            {
                continue;
            }

            await strategy.HandleMarketData(data, cancellationToken);
        }
    }

    private List<StrategyBase> InitStrategies()
    {
        var repo = ServiceProvider.GetRequiredService<IStrategyMetadataRepository>();

        var res = new List<StrategyBase>();

        var strategiesMeta = repo.GetAll();

        foreach (var metadata in strategiesMeta)
        {
            var strategy = StrategyFactory.Create(metadata, ServiceProvider);
            res.Add(strategy);
        }

        return res;
    }
}
