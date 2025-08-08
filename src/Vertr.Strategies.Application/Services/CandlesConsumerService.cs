using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Channels;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application.Services;

internal class CandlesConsumerService : DataConsumerServiceBase<Candle>
{
    private readonly IStrategyHostingService _strategyHostingService;
    private readonly ILogger<CandlesConsumerService> _logger;

    public CandlesConsumerService(
        IServiceProvider serviceProvider,
        ILogger<CandlesConsumerService> logger) : base(serviceProvider)
    {
        _strategyHostingService = serviceProvider.GetRequiredService<IStrategyHostingService>();
        _logger = logger;
    }

    protected override async Task Handle(Candle data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"New Candle received: InstrumentId={data.InstrumentId}");

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
