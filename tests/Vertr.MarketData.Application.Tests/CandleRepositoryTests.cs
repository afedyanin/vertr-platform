using Vertr.MarketData.Application.Repositories;

namespace Vertr.MarketData.Application.Tests;

public class CandleRepositoryTests
{
    private static readonly DateTime _startDate = new DateTime(2025, 07, 07);

    [Test]
    public void CanAddRange()
    {
        var repo = new CandleRepository(3);
        var candles = CandleFactory.CreateCandles(_startDate, 200, Guid.NewGuid());

        repo.AddRange(candles);
        var last = repo.GetLast();

        Assert.That(last, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(repo.IsFull, Is.True);
            Assert.That(last.TimeUtc, Is.EqualTo(candles[199].TimeUtc));
        });
    }
}
