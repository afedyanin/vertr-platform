using System.Text.Json;
using Vertr.Common.ForecastClient.Models;

namespace Vertr.Common.ForecastClient.Tests;

public class ModelTests
{
    [Test]
    public void CanSerializeSeriesItem()
    {
        // 2020-11-01T14:45:00
        var time = DateTime.UtcNow;

        var item = new SeriesItem()
        {
            Ticker = "SBER",
            Time = time.ToString("yyyy-MM-ddTHH:mm:ss"),
            Value = 123.456m
        };

        var json = JsonSerializer.Serialize(item);

        Assert.That(json, Is.Not.Null);

        Console.WriteLine(json);
    }

    [Test]
    public void CanSerialzeForecastRequest()
    {
        var request = new ForecastRequest()
        {
            Models =
            [
                "naive",
                "auto_arima",
                "auto_ets",
                "auto_ces",
                "auto_theta",
                "random_walk",
                "history_average"
            ],

            Series = ForecastApiTests.GenerateSeries("SBER", DateTime.UtcNow).ToArray()
        };

        var json = JsonSerializer.Serialize(request);

        Assert.That(json, Is.Not.Null);

        Console.WriteLine(json);
    }
}
