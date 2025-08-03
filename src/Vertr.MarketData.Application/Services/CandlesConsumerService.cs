using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.MarketData.Application.Services;

internal class CandlesConsumerService : DataConsumerServiceBase<Candle>
{
    private readonly IStrategyHostingService _strategyHostingService;

    public CandlesConsumerService(
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _strategyHostingService = serviceProvider.GetRequiredService<IStrategyHostingService>();
    }

    protected override async Task Handle(Candle data, CancellationToken cancellationToken = default)
    {
        var activeStrategies = await _strategyHostingService.GetActiveStrategies();

        foreach (var strategy in activeStrategies)
        {
            if (strategy.InstrumentId != data.InstrumentId)
            {
                continue;
            }

            await strategy.HandleMarketData(data, cancellationToken);
        }
    }
}
