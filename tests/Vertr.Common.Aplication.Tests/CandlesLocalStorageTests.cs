using Refit;
using Vertr.Common.Application.LocalStorage;
using Vertr.Common.Contracts;

namespace Vertr.Common.Aplication.Tests;

public class CandlesLocalStorageTests
{
    private static readonly Guid SberId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");

    [Test]
    public void CanUpdateCandle()
    {
        var storage = new CandlesLocalStorage();
        var candles = CreateCandles(SberId, 1);
        storage.Update(candles[0]);

        Assert.That(storage.GetCount(SberId), Is.EqualTo(1));
    }

    private static Candle[] CreateCandles(Guid instrumentId, [Query] long maxItems = -1)
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

        return [.. res];
    }
}
