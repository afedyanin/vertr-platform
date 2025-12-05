using System.Text;
using System.Text.Json;
using Vertr.CommandLine.Models;
using Vertr.CommandLine.Models.Requests.Predictor;
using Vertr.CommandLine.Predictor.Client.Convertors;

namespace Vertr.CommandLine.Predictor.Client.Tests;

[TestFixture(Category = "Web", Explicit = true)]
public class PredictorClientTests : ClientTestBase
{
    private static readonly Candle Candle1 = new Candle
    {
        TimeUtc = DateTime.UtcNow.AddHours(-1),
        Open = 77.45m,
        High = 85.13m,
        Low = 71.14m,
        Close = 82.34m,
        Volume = 46654
    };

    private static readonly Candle Candle2 = new Candle
    {
        TimeUtc = DateTime.UtcNow,
        Open = 77.45m,
        High = 85.13m,
        Low = 71.14m,
        Close = 100.2m,
        Volume = 46654
    };

    [Test]
    public async Task CanPredictByLastValue()
    {
        var candles = new Candle[] { Candle1, Candle2 };
        var response = await PredictorClient.Naive(candles.ToRequest());

        Assert.That(response, Is.Not.Null);

        Console.WriteLine(response);
    }

    [Test]
    public async Task CanPredictRandomWalk()
    {
        var candles = new Candle[] { Candle1, Candle2 };

        var request = candles.ToRequest();

        var requestJson = JsonSerializer.Serialize(request);
        Console.WriteLine($"{requestJson}");

        var response = await PredictorClient.RandomWalkWithDrift(request);

        Assert.That(response, Is.Not.Null);

        Console.WriteLine(response);
    }

    [Test]
    public async Task CanPredictByLastValueViaPredictionService()
    {
        var data = new Dictionary<string, object>
        {
            [PredictionContextKeys.Candles] = new Candle[] { Candle1, Candle2 }
        };

        var response = await PredictionService.Predict(DateTime.UtcNow, "SBER", PredictorType.Naive, data);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Count, Is.GreaterThan(0));

        Console.WriteLine(Dump(response));
    }

    [Test]
    public async Task CanPredictRandomWalkViaPredictionService()
    {
        var data = new Dictionary<string, object>
        {
            [PredictionContextKeys.Candles] = new Candle[] { Candle1, Candle2 }
        };

        var response = await PredictionService.Predict(DateTime.UtcNow, "SBER", PredictorType.RandomWalkWithDrift, data);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Count, Is.GreaterThan(0));

        Console.WriteLine(Dump(response));
    }

    [Test]
    public async Task CanPredictAutoArimaViaPredictionService()
    {
        var data = new Dictionary<string, object>
        {
            [PredictionContextKeys.Candles] = new Candle[] { Candle1, Candle2 }
        };

        var response = await PredictionService.Predict(DateTime.UtcNow, "SBER", PredictorType.AutoArima, data);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Count, Is.GreaterThan(0));

        Console.WriteLine(Dump(response));
    }

    private string Dump(Dictionary<string, object> dict)
    {
        var sb = new StringBuilder();

        foreach (var kvp in dict)
        {
            sb.AppendLine($"{kvp.Key} = {kvp.Value}");
        }

        return sb.ToString();
    }
}