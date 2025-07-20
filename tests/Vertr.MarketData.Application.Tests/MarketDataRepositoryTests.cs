using Vertr.MarketData.Application.Repositories;

namespace Vertr.MarketData.Application.Tests;

public class MarketDataRepositoryTests
{
    private static readonly DateTime _startDate = new DateTime(2025, 07, 07);

    private static readonly Guid _sber = Guid.NewGuid();
    private static readonly Guid _sberp = Guid.NewGuid();

    [Test]
    public void CanCreateRepository()
    {
        var repo = new MarketDataRepository(5);
        var candles_sber = CandleFactory.CreateCandles(_startDate, 200, _sber);
        var candles_sberp = CandleFactory.CreateCandles(_startDate.AddDays(12), 200, _sberp);

        repo.AddRange(candles_sber);
        repo.AddRange(candles_sberp);

        var last_sber = repo.GetLast(_sber);
        var last_sberp = repo.GetLast(_sberp);

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
