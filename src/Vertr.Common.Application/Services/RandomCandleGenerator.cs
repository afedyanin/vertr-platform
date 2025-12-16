using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

public static class RandomCandleGenerator
{
    public static Candle[] GetRandomCandles(
        Guid instrumentId,
        DateTime startTime,
        decimal initialPrice,
        TimeSpan timeSpan,
        int count = 100)
    {
        var res = new List<Candle>();
        var currentTime = startTime;
        var price = initialPrice;

        for (var i = 0; i < count; i++)
        {
            var candle = GetRandomCandle(instrumentId, currentTime, price);
            res.Add(candle);

            currentTime += timeSpan;
            price = candle.Close;
        }

        return [.. res];
    }

    public static Candle GetRandomCandle(
        Guid instrumentId,
        DateTime time,
        decimal initialPrice)
    {
        var decimals = GetRandomDecimals(initialPrice);

        var candle = new Candle
        (
            InstrumentId: instrumentId,
            TimeUtc: time,
            Open: decimals.First(),
            Close: decimals.Last(),
            High: decimals.Max(),
            Low: decimals.Min(),
            Volume: 0L
        );

        return candle;
    }

    public static decimal[] GetRandomDecimals(decimal initialValue, int count = 100)
    {
        var decimals = new decimal[count];
        var current = initialValue;

        for (var i = 0; i < count; i++)
        {
            current = GetNextValue(current);
            decimals[i] = current;
        }

        return decimals;
    }

    public static decimal GetNextValue(decimal valueToChange, double rangePercent = 0.05)
    {
        var percentChange = Random.Shared.NextDouble() * rangePercent;
        var pos = Random.Shared.NextDouble() > 0.51;
        var change = Math.Round(valueToChange * (decimal)percentChange, 2);
        change = pos ? change : -change;
        valueToChange += change;

        return valueToChange;
    }
}
