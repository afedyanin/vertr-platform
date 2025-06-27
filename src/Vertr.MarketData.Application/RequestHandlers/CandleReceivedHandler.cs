using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Requests;

namespace Vertr.MarketData.Application.RequestHandlers;

internal class CandleReceivedHandler : IRequestHandler<CandleReceivedRequest>
{
    private readonly ILogger<CandleReceivedHandler> _logger;

    public CandleReceivedHandler(ILogger<CandleReceivedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CandleReceivedRequest request, CancellationToken cancellationToken)
    {
        var candle = request.Candle;

        if (candle != null)
        {
            // TODO: Save Candle to Redis
            //_logger.LogInformation($"New candle received {request.InstrumentIdentity.Ticker} ({request.Interval}): Time={candle.TimeUtc:O} Close={candle.Close}");
        }

        return Task.CompletedTask;
    }
}
