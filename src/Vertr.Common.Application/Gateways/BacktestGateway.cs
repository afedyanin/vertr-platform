using Vertr.Common.Application.Abstractions;
using Vertr.Common.Application.Services;
using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Gateways;

internal sealed class BacktestGateway : ITradingGateway
{
    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

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
    ];

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
        // TODO: Implement this
        return Task.CompletedTask;
    }
}
