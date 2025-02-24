using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.Adapters.Prediction.Converters;
using Vertr.Adapters.Prediction.Models;
using Vertr.Domain;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Prediction.Tests;

[TestFixture(Category = "integration", Explicit = true)]
public class PredictionServiceTests
{
    [Test]
    public async Task CanSendPredictionRequest()
    {
        var predictionApi = RestService.For<IPredictionApi>("http://127.0.0.1:8081");

        var request = new PredictionRequest
        {
            Symbol = "SBER",
            Interval = (int)CandleInterval._10Min,
            Predictor = PredictorType.Sb3.Name,
            Algo = Sb3Algo.DQN.Name,
            CandlesCount = 20,
            CompletedCandelsOnly = true,
            CandlesSource = "tinvest"
        };

        var response = await predictionApi.Predict(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.Time, Has.Length.GreaterThan(0));
            Assert.That(response.Action, Has.Length.GreaterThan(0));
            Assert.That(response.Time, Has.Length.EqualTo(response.Action.Length));
        });

        var items = response.Convert();

        foreach (var item in items)
        {
            Console.WriteLine($"{item.Item1:O} : {item.Item2}");
        }
    }

    [Test]
    public async Task CanUsePredictionService()
    {
        var predictionApi = RestService.For<IPredictionApi>("http://127.0.0.1:8081");
        IPredictionService service = new PredictionService(predictionApi);

        var items = await service.Predict("SBER", CandleInterval._10Min, PredictorType.Sb3, Sb3Algo.DQN);

        Assert.That(items, Is.Not.Null);

        foreach (var item in items)
        {
            Console.WriteLine($"{item.Item1:O} : {item.Item2}");
        }
    }

    [Test]
    public void CanUsePredictionServiceFromDi()
    {
        var configuration = ConfigFactory.GetConfiguration();
        var predictionSettings = new PredictionSettings();
        configuration.GetSection(nameof(PredictionSettings)).Bind(predictionSettings);

        Assert.That(predictionSettings.BaseAddress, Is.EqualTo("http://127.0.0.1:8081"));

        var services = new ServiceCollection();
        services.AddPredictions(c => c.BaseAddress = new Uri(predictionSettings.BaseAddress));
        var serviceProvider = services.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IPredictionService>();
        Assert.That(service, Is.Not.Null);
    }
}
