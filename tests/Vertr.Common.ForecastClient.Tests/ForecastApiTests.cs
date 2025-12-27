using Refit;
using Vertr.Common.ForecastClient.Models;

namespace Vertr.Common.ForecastClient.Tests;

public class ForecastApiTests
{
    private const string BaseUrl = "http://localhost:8081";

    private IVertrForecastClient _api;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _api = RestService.For<IVertrForecastClient>(BaseUrl);
    }

    [Test]
    public async Task CanGetKeysStats()
    {
        var allKeys = await _api.GetKeysStats();
        Assert.That(allKeys, Is.Not.Empty);
        Console.WriteLine(string.Join(',', allKeys));
    }

    [Test]
    public async Task CanForecastStats()
    {
        var request = new ForecastRequest()
        {
            Models = StatModelNames,
            Series = GenerateSeries("SBER", DateTime.UtcNow).ToArray()
        };

        var response = await _api.ForecastStats(request);

        Assert.That(response, Is.Not.Empty);
        Console.WriteLine(string.Join("\n", response));
    }

    [Test]
    public async Task CanForecastMl()
    {
        var request = new ForecastRequest()
        {
            Models = MlModelNames,
            Series = GenerateSeries("SBER", DateTime.UtcNow).ToArray()
        };

        var response = await _api.ForecastMl(request);

        Assert.That(response, Is.Not.Empty);
        Console.WriteLine(string.Join("\n", response));
    }

    [Test]
    public async Task CanForecastAutoLSTM()
    {
        var series = GenerateSeries("SBER", DateTime.UtcNow).ToArray();
        var response = await _api.AutoLSTM(series);
        Console.WriteLine(response);
    }

    public string[] StatModelNames =>
        [
            "naive",
            "auto_arima",
            "auto_ets",
            "auto_ces",
            "auto_theta",
            "random_walk",
            "history_average"
        ];

    public string[] MlModelNames =>
        [
            "lgbm",
            "lasso",
            "lin_reg",
            "ridge",
            "knn",
            "mlp",
            "rf"
        ];

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
