using System.Text.Json;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;

namespace Vertr.MarketData.DataAccess.Tests.Repositories;

[TestFixture(Category = "Database", Explicit = true)]
public class CandlesRepositoryTests : RepositoryTestBase
{
    [TestCase("sample_data\\candles.json")]
    public async Task CanInsertCandles(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        var candles = JsonSerializer.Deserialize<Candle[]>(json, JsonOptions.DefaultOptions) ?? [];

        var savedCount = await CandlesRepo.Upsert(candles);
        Assert.That(savedCount, Is.GreaterThan(0));
    }

    [TestCase("e6123145-9665-43e0-8413-cd61b8aa9b13")]
    public async Task CanGetCandles(string instrumentId)
    {
        var candles = await CandlesRepo.Get(Guid.Parse(instrumentId), limit: 50);
        Assert.That(candles.Count, Is.GreaterThan(0));
        Assert.That(candles.Count, Is.LessThanOrEqualTo(50));
    }

    [TestCase("e6123145-9665-43e0-8413-cd61b8aa9b13")]
    public async Task CanGetLastCandle(string instrumentId)
    {
        var candle = await CandlesRepo.GetLast(Guid.Parse(instrumentId));
        Assert.That(candle, Is.Not.Null);
    }
}
