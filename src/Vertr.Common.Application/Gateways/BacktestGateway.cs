using Refit;
using Vertr.Common.Application.Abstractions;
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

    public Task<Candle[]> GetCandles(Guid instrumentId, [Query] long maxItems = -1)
    {
        var startTime = DateTime.UtcNow.AddHours(-3);
        var maxCount = maxItems > 0 ? maxItems : 100;

        var res = new List<Candle>();

        for (var i = 0; i < maxCount; i++)
        {
            var candle = new Candle
            (
                InstrumentId: instrumentId,
                TimeUtc: startTime.AddMinutes(i),
                Open: 90.0m,
                Close: 95.45m,
                High: 101.14m,
                Low: 89.89m,
                Volume: 1234
            );

            res.Add(candle);
        }

        return Task.FromResult(res.ToArray());
    }

    public Task PostMarketOrder(MarketOrderRequest request)
    {
        return Task.CompletedTask;
    }
}
