using System.Text;
using Vertr.CommandLine.Models.Helpers;

namespace Vertr.CommandLine.Models.Tests.Helpers;

public class CandleExtensionsTests
{

    [Test]
    public void CandGetCandlesEqualOrLessThanBefore()
    {
        var startTime = new DateTime(2025, 11, 7);
        var step = TimeSpan.FromHours(1);
        var before = startTime.AddHours(5).AddMinutes(2);

        var candels = CreateCandles(13, startTime, step);
        Console.WriteLine($"Base:\n{Dump(candels)}");

        var selected = candels.GetEqualOrLessThanBefore(before, 8);
        Console.WriteLine($"\nSelected before={before:s}:\n{Dump(selected)}");

        Assert.Pass();
    }

    [Test]
    public void CanGetCandlesEqualOrGreatherThanAfter()
    {
        var startTime = new DateTime(2025, 11, 7);
        var step = TimeSpan.FromHours(1);
        var after = startTime.AddHours(5).AddMinutes(2);

        var candels = CreateCandles(13, startTime, step);
        Console.WriteLine($"Base:\n{Dump(candels)}");

        var selected = candels.GetEqualOrGreatherThanAfter(after, 8);
        Console.WriteLine($"\nSelected after={after:s}:\n{Dump(selected)}");

        Assert.Pass();
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(13)]
    [TestCase(-1)]
    [TestCase(-3)]
    [TestCase(-13)]
    public void CanGetGetShiftedCandle(int shift)
    {
        var startTime = new DateTime(2025, 11, 7);
        var step = TimeSpan.FromHours(1);
        var time = startTime.AddHours(5);

        var candels = CreateCandles(13, startTime, step);
        Console.WriteLine($"Base:\n{Dump(candels)}");

        var selected = candels.GetShifted(time, shift);

        Console.WriteLine($"\nSelected time={time:s}:\n{Dump([selected!])}");

        Assert.Pass();
    }

    [Test]
    public void CanGroupCandlesByDay()
    {
        var startTime = new DateTime(2025, 11, 7);
        var step = TimeSpan.FromHours(5);

        var candels = CreateCandles(41, startTime, step);
        Console.WriteLine($"Base:\n{Dump(candels)}");

        var grouped = candels.GetRanges("SBER");

        foreach (var kvp in grouped)
        {
            Console.WriteLine($"{kvp.Key}={kvp.Value}");
        }
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