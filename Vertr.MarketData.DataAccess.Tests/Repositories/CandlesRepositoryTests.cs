using System.Text.Json;
using Vertr.MarketData.Contracts;

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
}
