using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Vertr.Moex.ApiClient;

public static class MoexApiClientRegistrar
{
    public static IServiceCollection AddMoexApiClient(this IServiceCollection services, string baseAddress)
    {
        services
           .AddRefitClient<IMoexApiClient>(
               new RefitSettings
               {
                   ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions.DefaultOptions)
               })
           .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

        return services;
    }
}
