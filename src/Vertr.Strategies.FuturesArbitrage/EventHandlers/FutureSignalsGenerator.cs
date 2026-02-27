using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;
using Vertr.Strategies.FuturesArbitrage.Models;

namespace Vertr.Strategies.FuturesArbitrage.EventHandlers;

internal sealed class FutureSignalsGenerator : IEventHandler<OrderBookChangedEvent>
{
    private readonly IOrderBooksLocalStorage _orderBooksLocalStorage;
    private readonly IInstrumentsLocalStorage _instrumentsLocalStorage;
    private readonly ILogger<FutureSignalsGenerator> _logger;
    private readonly IPortfoliosLocalStorage _portfolioRepository;

    private const double Threshold = 0.0001;

    public int HandlingOrder => 30;

    public FutureSignalsGenerator(
        IOrderBooksLocalStorage orderBooksLocalStorage,
        IInstrumentsLocalStorage instrumentsLocalStorage,
        IPortfoliosLocalStorage portfolioRepository,
        ILogger<FutureSignalsGenerator> logger)
    {
        _orderBooksLocalStorage = orderBooksLocalStorage;
        _instrumentsLocalStorage = instrumentsLocalStorage;
        _portfolioRepository = portfolioRepository;
        _logger = logger;
    }

    public async ValueTask OnEvent(OrderBookChangedEvent data)
    {
        if (data.TradingDirection == TradingDirection.Hold)
        {
            return;
        }

        foreach (var kvp in data.FairPrices)
        {
            if (!kvp.Value.HasValue)
            {
                _logger.LogWarning("#{Sequence} FairPrice is not found for InstrumentId={InstrumentId}", data.Sequence, kvp.Key);
                continue;
            }

            var orderBook = _orderBooksLocalStorage.GetById(kvp.Key);

            if (orderBook == null)
            {
                _logger.LogWarning("#{Sequence} OrderBook is not found for InstrumentId={InstrumentId}", data.Sequence, kvp.Key);
                continue;
            }

            var instrument = _instrumentsLocalStorage.GetById(kvp.Key);
            if (instrument == null)
            {
                _logger.LogWarning("#{Sequence} Instrument is not found for InstrumentId={InstrumentId}", data.Sequence, kvp.Key);
                continue;
            }

            var fairPrice = kvp.Value.Value;
            var quote = new Quote
            {
                Bid = orderBook.MaxBid,
                Ask = orderBook.MinAsk,
            };

            _logger.LogDebug("#{Sequence} Fair={FairPrice:N4} B={Bid:N4} A={Ask:N4}", data.Sequence, fairPrice, quote.Bid, quote.Ask);

            var direction = GetTradingDirection(fairPrice, quote, Threshold);

            if (direction == TradingDirection.Hold)
            {
                continue;
            }

            if (data.TradingDirection != direction)
            {
                continue;
            }

            var signal = new TradingSignal
            {
                Direction = direction,
                Instrument = instrument,
                PortfolioName = instrument.Ticker,
            };

            data.TradingSignals.Add(signal);

            var assetInfo = new DerivedAssetInfo
            {
                InstrumentId = instrument.Id,
                PortfolioId = _portfolioRepository.GetByName(instrument.Ticker)?.Id,
                UpdatedAt = orderBook.UpdatedAt,
                FairPrice = fairPrice,
                MaxBid = orderBook.MaxBid,
                MinAsk = orderBook.MinAsk,
                Threshold = Threshold,
                Direction = direction,
            };

            data.DerivedAssets.Add(assetInfo);
        }

        _logger.LogInformation("#{Sequence} Signals={SignalCount}", data.Sequence, data.TradingSignals.Count);
    }

    internal static TradingDirection GetTradingDirection(decimal fairPrice, Quote marketQuote, double threshold)
    {
        Debug.Assert(marketQuote.Ask >= marketQuote.Bid);

        // цена будет выше минимальной цены предложения
        var askDelta = (double)((fairPrice - marketQuote.Ask) / marketQuote.Ask);
        if (askDelta >= threshold)
        {

            return TradingDirection.Buy;
        }

        // цена будет ниже максимальной цены спроса
        var bidDelta = (double)((marketQuote.Bid - fairPrice) / fairPrice);
        return bidDelta >= threshold ? TradingDirection.Sell : TradingDirection.Hold;
    }
}
