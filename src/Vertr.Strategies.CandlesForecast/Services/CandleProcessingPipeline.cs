using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Application.LocalStorage;
using Vertr.Common.Contracts;
using Vertr.Strategies.CandlesForecast.Abstractions;

namespace Vertr.Strategies.CandlesForecast.Services;

internal sealed class CandleProcessingPipeline : ICandleProcessingPipeline
{
    private readonly ITradingGateway _tradingGateway;
    private readonly ICandlesLocalStorage _candlesLocalStorage;
    private readonly ILogger<CandleProcessingPipeline> _logger;
    private readonly IEventHandler<CandleReceivedEvent>[] _pipeline = [];

    private Instrument[] _instruments = [];

    public CandleProcessingPipeline(
        ITradingGateway tradingGateway,
        ICandlesLocalStorage candlesLocalStorage,
        IEnumerable<IEventHandler<CandleReceivedEvent>> eventHandlers,
        ILoggerFactory loggerFactory)
    {
        _tradingGateway = tradingGateway;
        _candlesLocalStorage = candlesLocalStorage;
        _logger = loggerFactory.CreateLogger<CandleProcessingPipeline>();
        _pipeline = [.. eventHandlers.OrderBy(h => h.HandlingOrder)];
    }

    public async Task Handle(CandleReceivedEvent tEvent)
    {
        try
        {
            var candle = tEvent.Candle;

            tEvent.Instrument = _instruments.GetById(candle.InstrumentId);

            await LoadHistoricCandlesIfReq(candle.InstrumentId);

            _candlesLocalStorage.Update(candle);

            foreach (var handler in _pipeline)
            {
                await handler.OnEvent(tEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event={Event} Message={Message}", tEvent, ex.Message);
        }
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        _instruments = await _tradingGateway.GetAllInstruments();
    }

    private async ValueTask LoadHistoricCandlesIfReq(Guid instrumentId)
    {
        if (_candlesLocalStorage.Any(instrumentId))
        {
            return;
        }

        var historicCandles = await _tradingGateway.GetCandles(instrumentId, CandlesLocalStorage.CandlesBufferLength);
        _candlesLocalStorage.Fill(historicCandles);
    }
}
