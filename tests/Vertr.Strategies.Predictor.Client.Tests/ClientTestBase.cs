using Microsoft.Extensions.DependencyInjection;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Predictor.Client.Tests;

public abstract class ClientTestBase
{
    private const string _baseAddress = "http://127.0.0.1:8081";

    private readonly IServiceProvider _serviceProvider;

    public IPredictorClient PredictorClient { get; init; }

    public IPredictionService PredictionService { get; init; }

    protected ClientTestBase()
    {
        var services = new ServiceCollection();
        services.AddPredictionService(_baseAddress);
        _serviceProvider = services.BuildServiceProvider();

        PredictorClient = _serviceProvider.GetRequiredService<IPredictorClient>();
        PredictionService = _serviceProvider.GetRequiredService<IPredictionService>();
    }
}
