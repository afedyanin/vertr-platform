using System.Text;
using System.Text.Json;
using Vertr.MarketData.Contracts;
using Vertr.Platform.Common.Utils;

namespace Vertr.MarketData.DataAccess.Tests.Repositories;

[TestFixture(Category = "Database", Explicit = true)]
public class CandlesHistoryRepsitoryTests : RepositoryTestBase
{
    [Test]
    public async Task CanSaveEmptyHistory()
    {
        var item = new CandlesHistoryItem
        {
            Id = Guid.NewGuid(),
            InstrumentId = Guid.Parse("e6123145-9665-43e0-8413-cd61b8aa9b13"),
            Day = DateOnly.FromDateTime(DateTime.UtcNow),
            Interval = CandleInterval.Min_1,
            Count = 0,
            Data = []
        };

        var saved = await CandlesHistoryRepo.Save(item);
        Assert.That(saved, Is.True);
    }

    [TestCase("sample_data\\candles.json")]
    public async Task CanSaveDailyCandles(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        var candles = JsonSerializer.Deserialize<Candle[]>(json, JsonOptions.DefaultOptions);

        Assert.That(candles, Is.Not.Empty);

        var first = candles[0];

        var item = new CandlesHistoryItem
        {
            Id = Guid.NewGuid(),
            InstrumentId = first.InstrumentId,
            Day = DateOnly.FromDateTime(first.TimeUtc),
            Interval = CandleInterval.Min_1,
            Count = candles.Length,
            Data = Encoding.UTF8.GetBytes(json)
        };

        var saved = await CandlesHistoryRepo.Save(item);
        Assert.That(saved, Is.True);
    }
}
