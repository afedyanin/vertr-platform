using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class FuturePriceCalculationHandler : IEventHandler<OrderBookChangedEvent>
{
    private readonly IInstrumentsLocalStorage _instrumentsLocalStorage;
    private readonly ILogger<FuturePriceCalculationHandler> _logger;

    public int HandlingOrder => 20;

    public FuturePriceCalculationHandler(
        IInstrumentsLocalStorage instrumentsLocalStorage,
        ILogger<FuturePriceCalculationHandler> logger)
    {
        _logger = logger;
        _instrumentsLocalStorage = instrumentsLocalStorage;
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

            var instrument = await _instrumentsLocalStorage.GetById(instrumentId);
            Debug.Assert(instrument != null);

            var spot = data.OrderBook.MidPrice;
            var rusfar = 15.45m / 100;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var expDate = new DateOnly(2026, 6, 15);
            var lotSize = 100;

            var daysToExpiry = GetDaysToExpiry(today, expDate);
            if (daysToExpiry <= 0)
            {
                _logger.LogWarning("#{Sequence}. Future={Ticker} is expired.", data.Sequence, instrument.Ticker);
            }

            var fairPrice = GetFairPrice(spot, rusfar, daysToExpiry);
            data.FairPrices[instrumentId] = fairPrice * lotSize;

            _logger.LogDebug("#{Sequence}. Future={Ticker} FairPrice={FairPrice}.", data.Sequence, instrument.Ticker, fairPrice);
        }
    }

    private static decimal GetFairPrice(decimal spotPrice, decimal rate, int daysToExpiry)
        => spotPrice * (1 + (rate / 100) * (daysToExpiry / 365));

    private int GetDaysToExpiry(DateOnly today, DateOnly expDate)
        => (expDate.DayNumber - today.DayNumber);
}
