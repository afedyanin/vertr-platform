using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Vertr.Clients.ForecastApiClient;

public static class ForecastClientRegistrar
{
    public static IServiceCollection AddForecastApiClient(this IServiceCollection services, string baseAddress)
    {
        services
           .AddRefitClient<IForecastApiClient>(
               new RefitSettings
               {
                   ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions.DefaultOptions)
               })
           .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

        return services;
    }
}
