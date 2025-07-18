using Vertr.MarketData.Application.Repositories;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Tests;

public class MarketDataRepositoryTests
{
    private static readonly DateTime _startDate = new DateTime(2025, 07, 07);

    private static readonly Symbol _sber = new Symbol("TQBR", "SBER");
    private static readonly Symbol _sberp = new Symbol("TQBR", "SBERP");
    private static readonly CandleInterval _1min = CandleInterval.Min_1;

    [Test]
    public void CanCreateRepository()
    {
        var repo = new MarketDataRepository(5);
        var candles_sber = CandleFactory.CreateCandles(_startDate, 200);
        var candles_sberp = CandleFactory.CreateCandles(_startDate.AddDays(12), 200);

        repo.AddRange(_sber, _1min, candles_sber);
        repo.AddRange(_sberp, _1min, candles_sberp);

        var last_sber = repo.GetLast(_sber, _1min);
        var last_sberp = repo.GetLast(_sberp, _1min);

        Assert.Multiple(() =>
        {
            Assert.That(last_sber, Is.Not.Null);
            Assert.That(last_sberp, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(last_sber.TimeUtc, Is.EqualTo(candles_sber[199].TimeUtc));
            Assert.That(last_sberp.TimeUtc, Is.EqualTo(candles_sberp[199].TimeUtc));
        });
    }
}
