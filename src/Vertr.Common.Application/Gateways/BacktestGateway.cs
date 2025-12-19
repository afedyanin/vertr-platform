using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Gateways;

internal sealed class BacktestGateway : ITradingGateway
{
    private const decimal CommissionPercent = 0.005m;

    private static readonly Instrument[] Instruments =
    [
        new Instrument
        {
            Id = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13"),
            Name = "Сбербанк",
            ClassCode = "TQBR",
            Ticker ="SBER",
            Currency ="rub",
            LotSize = 1,
            InstrumentType = "share"
        },
        new Instrument
        {
            Id = new Guid("a92e2e25-a698-45cc-a781-167cf465257c"),
            Name = "Российский рубль",
            ClassCode = "CETS",
            Ticker ="RUB",
            Currency ="rub",
            LotSize = 1,
            InstrumentType = "currency"
        },
    ];

    private readonly IPortfoliosLocalStorage _portfoliosLocalStorage;
    private readonly IMarketQuoteProvider _marketQuoteProvider;
    private readonly IHistoricCandlesProvider _historicCandlesProvider;

    public BacktestGateway(
        IPortfoliosLocalStorage portfoliosLocalStorage,
        IMarketQuoteProvider marketQuoteProvider,
        IHistoricCandlesProvider historicCandlesProvider)
    {
        _portfoliosLocalStorage = portfoliosLocalStorage;
        _marketQuoteProvider = marketQuoteProvider;
        _historicCandlesProvider = historicCandlesProvider;
    }

    public Task<Instrument[]> GetAllInstruments()
        => Task.FromResult(Instruments);

    public Task<Candle[]> GetCandles(Guid instrumentId, int maxItems = -1)
    {
        var candles = _historicCandlesProvider.Get(instrumentId, skip: 0, take: maxItems > 0 ? maxItems : 0);
        return Task.FromResult(candles.ToArray());
    }

    public Task PostMarketOrder(MarketOrderRequest request)
    {
        var portfolio = _portfoliosLocalStorage.GetById(request.PortfolioId);

        if (portfolio == null)
        {
            throw new InvalidOperationException($"Cannot find portfolio by PortfolioId={request.PortfolioId}");
        }

        var marketQuote = _marketQuoteProvider.GetMarketQuote(request.InstrumentId);

        if (marketQuote == null)
        {
            throw new InvalidOperationException($"Cannot get market quote by InstrumentId={request.InstrumentId}");
        }

        var lotSize = Instruments.GetLotSize(request.InstrumentId) ?? 1;

        var trades = new Trade[]
        {
            new Trade
            {
                TradeId = Guid.NewGuid().ToString(),
                Currency = "rub",
                Quantity = (long)(request.QuantityLots * lotSize),
                ExecutionTime = marketQuote.Value.Time,
                Price = marketQuote.Value.Mid,
            }
        };

        var tradesAmount = trades.Sum(t => t.Quantity * t.Price);
        var commisssionAmount = tradesAmount * CommissionPercent;

        var builder = new PortfolioBuilder(portfolio, Instruments);

        builder.ApplyComission(commisssionAmount, "rub");
        builder.ApplyTrades(request.InstrumentId, trades, request.Direction);

        portfolio = builder.Build();
        _portfoliosLocalStorage.Update(portfolio);

        return Task.CompletedTask;
    }
}
