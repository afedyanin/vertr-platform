using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.Adapters.Prediction.Converters;
using Vertr.Adapters.Prediction.Extensions;
using Vertr.Adapters.Prediction.Models;
using Vertr.Domain.Enums;
using Vertr.Domain.Ports;
using Vertr.Domain.Settings;

namespace Vertr.Adapters.Prediction.Tests;

[TestFixture(Category = "integration", Explicit = true)]
public class PredictionServiceTests
{
    private static readonly string _baseAddress = "http://127.0.0.1:8000";

    private static readonly StrategySettings _strategySettingsSb3 = new StrategySettings
    {
        Symbol = "SBER",
        Interval = CandleInterval._10Min,
        PredictorType = PredictorType.Sb3,
        Sb3Algo = Sb3Algo.DQN,
    };


    [TestCase(PredictorType.Sb3, Sb3Algo.DQN)]
    [TestCase(PredictorType.RandomWalk, Sb3Algo.Undefined)]
    [TestCase(PredictorType.TrendFollowing, Sb3Algo.Undefined)]
    public async Task CanSendPredictionRequest(PredictorType predictorType, Sb3Algo algo)
    {
        var predictionApi = RestService.For<IPredictionApi>(_baseAddress);

        var request = new PredictionRequest
        {
            Symbol = _strategySettingsSb3.Symbol,
            Interval = (int)_strategySettingsSb3.Interval,
            Predictor = predictorType.GetName(),
            Algo = algo.GetName(),
            CandlesCount = 120,
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
        var predictionApi = RestService.For<IPredictionApi>(_baseAddress);

        IPredictionService service = new PredictionService(predictionApi);

        var items = await service.Predict(_strategySettingsSb3);

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

        Assert.That(predictionSettings.BaseAddress, Is.EqualTo(_baseAddress));

        var services = new ServiceCollection();
        services.AddPredictions(c => c.BaseAddress = new Uri(predictionSettings.BaseAddress));
        var serviceProvider = services.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IPredictionService>();
        Assert.That(service, Is.Not.Null);
    }
}
