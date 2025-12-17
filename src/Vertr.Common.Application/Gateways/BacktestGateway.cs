using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Gateways;

internal sealed class BacktestGateway : ITradingGateway
{
    private readonly IPortfoliosLocalStorage _portfoliosLocalStorage;

    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");
    private static readonly Guid RubId = new Guid("a92e2e25-a698-45cc-a781-167cf465257c");

    private static readonly Instrument[] Instruments =
    [
        new Instrument
        {
            Id = SberId,
            Name = "Сбербанк",
            ClassCode = "TQBR",
            Ticker ="SBER",
            Currency ="rub",
            LotSize = 1,
            InstrumentType = "share"
        },
        new Instrument
        {
            Id = RubId,
            Name = "Российский рубль",
            ClassCode = "CETS",
            Ticker ="RUB",
            Currency ="rub",
            LotSize = 1,
            InstrumentType = "currency"
        },
    ];

    public BacktestGateway(IPortfoliosLocalStorage portfoliosLocalStorage)
    {
        _portfoliosLocalStorage = portfoliosLocalStorage;
    }

    public Task<Instrument[]> GetAllInstruments()
        => Task.FromResult(Instruments);

    public Task<Candle[]> GetCandles(Guid instrumentId, int maxItems = -1)
    {
        // TODO: Load data from CSV
        var count = maxItems < 0 ? 100 : maxItems;
        var candles = RandomCandleGenerator.GetRandomCandles(
            instrumentId,
            DateTime.UtcNow.AddDays(-1),
            100.0m,
            TimeSpan.FromMinutes(1),
            count);

        return Task.FromResult(candles.ToArray());
    }

    public Task PostMarketOrder(MarketOrderRequest request)
    {
        var portfolio = _portfoliosLocalStorage.GetById(request.PortfolioId);

        if (portfolio == null)
        {
            throw new InvalidOperationException($"Cannot find portfolio for OrderRequest={request}");
        }

        var builder = new PortfolioBuilder(portfolio, Instruments);

        // TODO: Implement this
        builder.ApplyComission(45.45m, "rub");

        var trades = new Trade[]
        {
            new Trade
            {
                // TODO: Implement this
                TradeId = Guid.NewGuid().ToString(),
                Currency = "rub",
                ExecutionTime = DateTime.UtcNow,
                Price = 200m,
                Quantity = Math.Abs(request.QuantityLots),
            }
        };

        // TODO: refactor
        var direction = request.QuantityLots > 0 ? TradingDirection.Buy : TradingDirection.Sell;

        builder.ApplyTrades(request.InstrumentId, trades, direction);
        portfolio = builder.Build();
        _portfoliosLocalStorage.Update(portfolio);

        return Task.CompletedTask;
    }
}
