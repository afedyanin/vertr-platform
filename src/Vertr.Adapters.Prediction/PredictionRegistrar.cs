using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Prediction;

public static class PredictionRegistrar
{
    public static IServiceCollection AddPredictions(
        this IServiceCollection services,
        Action<HttpClient> configureHttClient)
    {
        //services.AddSingleton(configuration);
        //services.AddOptions<PredictionSettings>().BindConfiguration(nameof(PredictionSettings));

        services
            .AddRefitClient<IPredictionApi>()
            .ConfigureHttpClient(c => configureHttClient(c));

        services.AddTransient<IPredictionService, PredictionService>();

        return services;
    }
}
