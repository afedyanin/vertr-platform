using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Application.Abstractions;

namespace Vertr.MarketData.Application.Commands;

internal class NewCandleHandler : IRequestHandler<NewCandleRequest>
{
    private readonly ICandlesRepository _candlesRepository;
    private readonly IMarketDataPublisher _marketDataPublisher;
    private readonly ILogger<NewCandleHandler> _logger;

    public NewCandleHandler(
        ICandlesRepository candlesRepository,
        IMarketDataPublisher marketDataPublisher,
        ILogger<NewCandleHandler> logger)
    {
        _candlesRepository = candlesRepository;
        _marketDataPublisher = marketDataPublisher;
        _logger = logger;
    }

    public async Task Handle(NewCandleRequest request, CancellationToken cancellationToken)
    {
        var candle = request.Candle;

        var saved = await _candlesRepository.Save(candle);

        if (!saved)
        {
            _logger.LogError($"Could not save candle: {candle}");
        }

        await _marketDataPublisher.Publish(candle, cancellationToken);
    }
}
