using Vertr.Common.Application.Extensions;
using Vertr.Common.Application.Services;

namespace Vertr.Common.Aplication.Tests;

public class RandomCandleGeneratorTests
{
    private static readonly Guid InstrumentId = Guid.NewGuid();

    [Test]
    public void CanGenerateDecimals()
    {
        var values = RandomCandleGenerator.GetRandomDecimals(100.0m, 200);

        Assert.That(values.Length, Is.EqualTo(200));

        var valuesString = string.Join("\n", values);
        Console.WriteLine(valuesString);
    }

    [Test]
    public void CanGenerateCandle()
    {
        var time = DateTime.UtcNow;
        var candle = RandomCandleGenerator.GetRandomCandle(InstrumentId, time, 100.06m);
        Console.WriteLine($"{candle}");
    }

    [Test]
    public void CanGenerateCandles()
    {
        var time = DateTime.UtcNow;
        var candles = RandomCandleGenerator.GetRandomCandles(InstrumentId, time, 100.06m, TimeSpan.FromHours(1));
        Console.WriteLine(candles.Dump());
    }
}
