using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Prediction;

public static class PredictionRegistrar
{
    public static IServiceCollection AddPredictions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddOptions<PredictionSettings>().BindConfiguration(nameof(PredictionSettings));
        services
            .AddRefitClient<IPredictionApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://127.0.0.1:8081"));


        services.AddInvestApiClient((_, settings) => configuration.Bind($"{nameof(TinvestSettings)}:{nameof(InvestApiSettings)}", settings));

        services.AddTransient<IPredictionService, PredictionService>();

        return services;
    }
}
