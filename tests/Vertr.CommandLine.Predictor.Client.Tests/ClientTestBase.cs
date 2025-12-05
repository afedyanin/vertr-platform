
using Microsoft.Extensions.DependencyInjection;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Predictor.Client.Tests;

public abstract class ClientTestBase
{
    private const string BaseAddress = "http://127.0.0.1:8081";

    private readonly IServiceProvider _serviceProvider;

    public IPredictionService PredictionService { get; init; }

    public IPredictorClient PredictorClient { get; init; }

    protected ClientTestBase()
    {
        var services = new ServiceCollection();
        services.AddPredictionService(BaseAddress);
        _serviceProvider = services.BuildServiceProvider();

        PredictorClient = _serviceProvider.GetRequiredService<IPredictorClient>();
        PredictionService = _serviceProvider.GetRequiredService<IPredictionService>();
    }
}