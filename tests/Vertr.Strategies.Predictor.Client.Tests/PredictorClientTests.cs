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
}
