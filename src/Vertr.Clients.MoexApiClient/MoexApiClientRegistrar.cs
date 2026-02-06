using Microsoft.Extensions.DependencyInjection;
using Vertr.Clients.MoexApiClient.Internal;

namespace Vertr.Clients.MoexApiClient;

public static class MoexApiClientRegistrar
{
    public static IServiceCollection AddMoexApiClient(this IServiceCollection services)
    {
        services.AddSingleton<IMoexApiClient, ApiClient>();

        return services;
    }
}
