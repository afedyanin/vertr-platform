using System.Text;
using Vertr.CommandLine.Application.Services;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Application.Tests.Services;

public class MarketDataServiceTests
{
    private const string DataFilePath = "Data\\SBER_251101_251104.csv";
    private const string Symbol = "SBER";

    [Test]
    public async Task CanCreateMarketDataService()
    {
        var startTime = new DateTime(2025, 11, 7);
        var step = TimeSpan.FromHours(1);
        var before = startTime.AddHours(5);
        var candels = CreateCandles(13, startTime, step);
        Console.WriteLine($"Base:\n{Dump(candels)}");

        var mds = new MarketDataService();
        await mds.LoadData(Symbol, candels);

        var selected = await mds.GetCandles(Symbol, before, 3);
        Assert.That(selected, Is.Not.Null);
        Console.WriteLine($"\nSelected before={before:s}:\n{Dump(selected)}");

        Assert.Pass();
    }

    [Test]
    public async Task CanEnumerateCandles()
    {
        var startTime = new DateTime(2025, 11, 7);
        var step = TimeSpan.FromHours(1);
        var candels = CreateCandles(13, startTime, step);
        Console.WriteLine($"Base:\n{Dump(candels)}");

        var mds = new MarketDataService();
        await mds.LoadData(Symbol, candels);

        var enumerable = await mds.GetEnumerable(Symbol);

        foreach (var time in enumerable)
        {
            Console.WriteLine(time.ToString("s"));
        }
    }

    [Test]
    public async Task CanGetLastMarketPrice()
    {
        var candles = CsvImporter.LoadCandles(DataFilePath);
        var mds = new MarketDataService();
        await mds.LoadData(Symbol, candles);

        var range = await mds.GetCandleRange(Symbol);
        Assert.That(range, Is.Not.Null);
        Console.WriteLine($"Candle range: {range}");

        var price = await mds.GetMarketPrice(Symbol, range.LastDate, PriceType.Mid);
        Assert.That(price, Is.Not.Null);
    }

    private static Candle[] CreateCandles(int count, DateTime startDate, TimeSpan step)
    {
        var res = new List<Candle>();
        var current = startDate;

        for (var i = 0; i < count; i++)
        {
            res.Add(new Candle { TimeUtc = current, });
            current += step;
        }

        return [.. res.OrderBy(c => c.TimeUtc)];
    }

    private static string Dump(IEnumerable<Candle> candles)
    {
        var sb = new StringBuilder();

        foreach (var candle in candles)
        {
            if (candle == null)
            {
                continue;
            }

            sb.AppendLine(candle.TimeUtc.ToString("s"));
        }

        return sb.ToString();
    }
}