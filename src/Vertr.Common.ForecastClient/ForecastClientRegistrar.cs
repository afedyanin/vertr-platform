using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Vertr.Common.ForecastClient;

public static class ForecastClientRegistrar
{
    public static IServiceCollection AddVertrForecastClient(this IServiceCollection services, string baseAddress)
    {
        services
           .AddRefitClient<IVertrForecastClient>(
               new RefitSettings
               {
                   ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions.DefaultOptions)
               })
           .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

        return services;
    }
}
