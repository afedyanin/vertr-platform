using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertr.MarketData.Application.CommandHandlers;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.Platform.Common.Utils;

namespace Vertr.MarketData.Application;

internal class CandlesHistoryLoader : ICandlesHistoryLoader
{
    private readonly IMarketDataGateway _marketDataGateway;
    private readonly ICandlesHistoryRepository _candlesRepository;
    private readonly MarketDataSettings _marketDataSettings;
    private readonly ILogger<LoadIntradayCandlesHandler> _logger;

    public CandlesHistoryLoader(
        IMarketDataGateway marketDataGateway,
        ICandlesHistoryRepository candlesRepository,
        IOptions<MarketDataSettings> marketDataSettings,
        ILogger<LoadIntradayCandlesHandler> logger)
    {
        _logger = logger;
        _marketDataGateway = marketDataGateway;
        _candlesRepository = candlesRepository;
        _marketDataSettings = marketDataSettings.Value;
    }

    public async Task<CandlesHistoryItem?> GetCandlesHistory(Guid instrumentId, DateOnly day, bool force = false)
    {
        var item = await _candlesRepository.GetByDay(instrumentId, day);

        if (item != null && !force)
        {
            return item;
        }

        item = await LoadCandlesHistoryForDay(instrumentId, day);

        return item;
    }

    private async Task<CandlesHistoryItem?> LoadCandlesHistoryForDay(Guid instrumentId, DateOnly day)
    {
        var candles = await _marketDataGateway.GetCandles(instrumentId, day, _marketDataSettings.CandleInterval);

        if (candles == null)
        {
            return null;
        }

        var item = new CandlesHistoryItem
        {
            Id = Guid.NewGuid(),
            InstrumentId = instrumentId,
            Day = day,
            Interval = _marketDataSettings.CandleInterval,
            Count = candles.Length,
            Data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(candles, JsonOptions.DefaultOptions)),
        };

        var saved = await _candlesRepository.Save(item);

        if (!saved)
        {
            _logger.LogError($"Cannot save candles history for day={day}.");
            return null;
        }

        return item;
    }
}
