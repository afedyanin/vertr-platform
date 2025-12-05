
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Predictor.Client;

public static class PredictionRegistrar
{
    public static IServiceCollection AddPredictionService(this IServiceCollection services, string predictorBaseUrl)
    {
        services.AddRefitClient<IPredictorClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(predictorBaseUrl));

        services.AddTransient<IPredictionService, PredictionService>();

        return services;
    }
}