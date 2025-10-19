using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Predictor.Client;
public static class PredictionServiceRegistrar
{
    public static IServiceCollection AddPredictionService(this IServiceCollection services, string predictorUrl)
    {
        services.AddRefitClient<IPredictorClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(predictorUrl));

        services.AddTransient<IPredictionService, PredictionService>();

        return services;
    }
}
