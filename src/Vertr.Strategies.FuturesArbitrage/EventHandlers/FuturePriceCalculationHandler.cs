using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class FuturePriceCalculationHandler : IEventHandler<OrderBookChangedEvent>
{
    private readonly IInstrumentsLocalStorage _instrumentsLocalStorage;
    private readonly IFutureInfoRepository _futureInfoRepository;
    private readonly IIndexRatesRepository _ratesRepository;

    private readonly ILogger<FuturePriceCalculationHandler> _logger;

    public int HandlingOrder => 20;

    public FuturePriceCalculationHandler(
        IInstrumentsLocalStorage instrumentsLocalStorage,
        IFutureInfoRepository futureInfoRepository,
        IIndexRatesRepository ratesRepository,
        ILogger<FuturePriceCalculationHandler> logger)
    {
        _logger = logger;
        _instrumentsLocalStorage = instrumentsLocalStorage;
        _futureInfoRepository = futureInfoRepository;
        _ratesRepository = ratesRepository;
    }

    public async ValueTask OnEvent(OrderBookChangedEvent data)
    {
        if (data.TradingDirection == Common.Contracts.TradingDirection.Hold)
        {
            return;
        }

        foreach (var instrumentId in data.FairPrices.Keys)
        {
            Debug.Assert(instrumentId != Guid.Empty);

            var instrument = _instrumentsLocalStorage.GetById(instrumentId);
            Debug.Assert(instrument != null);

            var futureInfo = _futureInfoRepository.Get(instrument.Ticker);

            if (futureInfo == null)
            {
                _logger.LogError("#{Sequence}. FutureInfo={Ticker} is not found.", data.Sequence, instrument.Ticker);
                continue;
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var expDate = futureInfo.ExpDate;
            var lotSize = futureInfo.LotSize;

            var daysToExpiry = GetDaysToExpiry(today, expDate);
            if (daysToExpiry <= 0)
            {
                _logger.LogError("#{Sequence}. Future={Ticker} is expired.", data.Sequence, instrument.Ticker);
                continue;
            }

            var symbol = daysToExpiry >= 45 ? "RUSFAR3M" : "RUSFAR";
            var rusfar = _ratesRepository.GetLast(symbol);
            if (rusfar == null)
            {
                _logger.LogError("#{Sequence}. RUSFAR rate is not found.", data.Sequence);
                continue;
            }

            var spot = data.OrderBook.MidPrice;
            var fairPrice = GetFairPrice(spot, rusfar.Value, daysToExpiry);
            data.FairPrices[instrumentId] = fairPrice * lotSize; // check it

            _logger.LogDebug("#{Sequence}. Future={Ticker} FairPrice={FairPrice}.", data.Sequence, instrument.Ticker, fairPrice);
        }
    }

    private static decimal GetFairPrice(decimal spotPrice, decimal rate, int daysToExpiry)
        => spotPrice * (1 + (rate / 100) * (daysToExpiry / 365));

    private int GetDaysToExpiry(DateOnly today, DateOnly expDate)
        => (expDate.DayNumber - today.DayNumber);
}
