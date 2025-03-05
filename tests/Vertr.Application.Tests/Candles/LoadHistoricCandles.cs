using Vertr.Domain.Enums;

namespace Vertr.Application.Tests.Candles;

[TestFixture(Category = "Integration", Explicit = true)]
public class LoadHistoricCandles : ApplicationTestBase
{
    [TestCase("SBER", CandleInterval._10Min, "2024.01.01", 5)]
    public async Task LoadAllCandles(string symbol, CandleInterval interval, string dateLimit, int step)
    {
        var limitDate = DateTime.Parse(dateLimit);
        var to = DateTime.UtcNow;

        while (to >= limitDate)
        {
            var from = to.AddDays(-step);
            var candles = await Gateway.GetCandles(symbol, interval, from, to);
            var inserted = await Repo.Insert(candles);
            Console.WriteLine($"{inserted} {symbol} rows inserted for {from:O} - {to:O}");
            to = from;
        }
    }
}
