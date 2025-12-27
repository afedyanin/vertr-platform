using Refit;
using Vertr.Common.ForecastClient.Models;

namespace Vertr.Common.ForecastClient.Tests;

public class ForecastApiTests
{
    [Test]
    public async Task CanGetAllKeys()
    {
        var api = RestService.For<IVertrForecastClient>("http://localhost:8081");

        var allKeys = await api.GetKeysStats();

        Assert.That(allKeys, Is.Not.Empty);
        Console.WriteLine(string.Join(',', allKeys));
    }

    [Test]
    public async Task CanForecastSomeValue()
    {
        var api = RestService.For<IVertrForecastClient>("http://localhost:8081");

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

            Series = GenerateSeries("SBER", DateTime.UtcNow).ToArray()
        };

        var response = await api.ForecastStats(request);

        Assert.That(response, Is.Not.Empty);
        Console.WriteLine(string.Join("\n", response));
    }

    public static IEnumerable<SeriesItem> GenerateSeries(string ticker, DateTime startTime, int count = 10)
    {
        var res = new List<SeriesItem>();

        for (var i = 0; i <= count; i++)
        {
            var item = new SeriesItem()
            {
                Ticker = ticker,
                Time = startTime.AddMinutes(i).ToString("yyyy-MM-ddTHH:mm:ss"),
                Value = 120.365m + i
            };

            res.Add(item);
        }

        return res;
    }
}
