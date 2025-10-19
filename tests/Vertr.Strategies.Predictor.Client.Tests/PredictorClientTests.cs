using System.Text;
using System.Text.Json;
using Microsoft.Data.Analysis;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Extensions;
using Vertr.Platform.Common.Utils;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Predictor.Client.Requests;

namespace Vertr.Strategies.Predictor.Client.Tests;

[TestFixture(Category = "Web", Explicit = true)]
public class PredictorClientTests : ClientTestBase
{
    private const string _csvFilePath = "Data\\dummy.csv";
    private const string _candlesFilePath = "Data\\candles.json";

    [Test]
    public async Task CanCallPredictorClient()
    {
        var csv = File.ReadAllText(_csvFilePath);

        var request = new PredictionRequest
        {
            ModelType = "Dummy",
            CsvContent = csv
        };

        var res = await PredictorClient.Predict(request);

        Assert.That(res, Is.Not.Null);

        Console.WriteLine(res.CsvContent);
    }

    [Test]
    public async Task CanCallPredictionService()
    {
        var csv = File.ReadAllText(_csvFilePath);
        var res = await PredictionService.Predict(StrategyType.Dummy, csv);

        Assert.That(res, Is.Not.Null);

        Console.WriteLine(res);
    }

    [Test]
    public void CanReadDataFrame()
    {
        var csv = File.ReadAllText(_csvFilePath);
        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var dataFrame = DataFrame.LoadCsv(csvStream);

        Assert.That(dataFrame, Is.Not.Null);
        Console.WriteLine(dataFrame);
    }

    [Test]
    public void CanSaveDataFrame()
    {
        var csv = File.ReadAllText(_csvFilePath);
        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var dataFrame = DataFrame.LoadCsv(csvStream);

        using var writeStream = new MemoryStream();
        DataFrame.SaveCsv(dataFrame, writeStream);
        var savedCsv = Encoding.UTF8.GetString(writeStream.ToArray());

        Assert.That(savedCsv, Is.Not.Empty);
        Console.WriteLine(savedCsv);
    }

    [Test]
    public async Task CanCallPredictionWithDataFrame()
    {
        var csv = File.ReadAllText(_csvFilePath);
        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var dataFrame = DataFrame.LoadCsv(csvStream);

        var df = await PredictionService.Predict(StrategyType.Dummy, dataFrame);

        Assert.That(df, Is.Not.Null);
        Console.WriteLine(df);
    }

    [Test]
    public async Task CanConvertCandlesToCsv()
    {
        var json = await File.ReadAllTextAsync(_candlesFilePath);
        var candles = JsonSerializer.Deserialize<Candle[]>(json, JsonOptions.DefaultOptions) ?? [];
        var csvString = candles.ToCsv();

        Assert.That(csvString, Is.Not.Null);
        Console.WriteLine(csvString);
    }

    [Test]
    public async Task CanCallPredictWithCandles()
    {
        var json = await File.ReadAllTextAsync(_candlesFilePath);
        var candles = JsonSerializer.Deserialize<Candle[]>(json, JsonOptions.DefaultOptions) ?? [];
        var df = await PredictionService.Predict(StrategyType.Dummy, candles);

        Assert.That(df, Is.Not.Null);
        Console.WriteLine(df);
    }
}
