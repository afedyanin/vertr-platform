using System.Text;
using Microsoft.Data.Analysis;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Predictor.Client.Requests;

namespace Vertr.Strategies.Predictor.Client.Tests;

[TestFixture(Category = "Web", Explicit = true)]
public class PredictorClientTests : ClientTestBase
{
    [Test]
    public async Task CanCallPredictorClient()
    {
        var csv = File.ReadAllText("Data\\dummy.csv");

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
        var csv = File.ReadAllText("Data\\dummy.csv");
        var res = await PredictionService.Predict(StrategyType.Dummy, csv);

        Assert.That(res, Is.Not.Null);

        Console.WriteLine(res);
    }

    [Test]
    public void CanReadDataFrame()
    {
        var csv = File.ReadAllText("Data\\dummy.csv");
        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var dataFrame = DataFrame.LoadCsv(csvStream);

        Assert.That(dataFrame, Is.Not.Null);
        Console.WriteLine(dataFrame);
    }

    [Test]
    public void CanSaveDataFrame()
    {
        var csv = File.ReadAllText("Data\\dummy.csv");
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
        var csv = File.ReadAllText("Data\\dummy.csv");
        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var dataFrame = DataFrame.LoadCsv(csvStream);

        var df = await PredictionService.Predict(StrategyType.Dummy, dataFrame);

        Assert.That(df, Is.Not.Null);
        Console.WriteLine(df);
    }

    [Test]
    public void CanConvertCandlesToDataFrame()
    {
        // TODO: Implement this
    }

    [Test]
    public void CanCallPredictWithMarketData()
    {
        // TODO:
        // Call marketData to get candles
        // Convert candles to dataFrame
        // Call prediction service
        // Check result
    }
}
