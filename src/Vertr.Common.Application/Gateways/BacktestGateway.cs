using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Extensions;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Gateways;

internal sealed class BacktestGateway : ITradingGateway
{
    private readonly IPortfoliosLocalStorage _portfoliosLocalStorage;

    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");
    private static readonly Guid RubId = new Guid("a92e2e25-a698-45cc-a781-167cf465257c");

    private const decimal CommissionPercent = 0.005m;

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

        var lotSize = Instruments.GetLotSize(request.InstrumentId) ?? 1;
        var trades = new Trade[]
        {
            new Trade
            {
                TradeId = Guid.NewGuid().ToString(),
                Currency = "rub",
                Quantity = (long)(request.QuantityLots * lotSize),
                
                // TODO: Use quote provider
                ExecutionTime = DateTime.UtcNow,
                Price = 200m,
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
