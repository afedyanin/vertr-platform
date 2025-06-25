using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.MarketData.Contracts.Requests;

namespace Vertr.MarketData.Application.RequestHandlers;

internal class NewCandleReceivedHandler : IRequestHandler<NewCandleReceived>
{
    private readonly ILogger<NewCandleReceivedHandler> _logger;

    public NewCandleReceivedHandler(ILogger<NewCandleReceivedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(NewCandleReceived request, CancellationToken cancellationToken)
    {
        var candle = request.Candle;

        if (candle != null)
        {
            // TODO: Save Candle to Redis
            _logger.LogInformation($"New candle received {request.InstrumentIdentity.Ticker} ({request.Interval}): Time={candle.TimeUtc:O} Close={candle.Close}");
        }

        return Task.CompletedTask;
    }
}
