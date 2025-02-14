using Microsoft.Extensions.DependencyInjection;

namespace Vertr.Adapters.Prediction;

public static class PredictionRegistrar
{
    public static IServiceCollection AddPredictions(this IServiceCollection services)
    {
        // TODO: register refit and service
        return services;
    }
}
