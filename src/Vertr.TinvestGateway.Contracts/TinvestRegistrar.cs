using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Vertr.TinvestGateway.Contracts;

public static class TinvestRegistrar
{
    public static IServiceCollection AddTinvestGateway(
        this IServiceCollection services,
        Action<HttpClient> configureHttClient)
    {
        services
              .AddRefitClient<ITinvestGateway>()
              .ConfigureHttpClient(c => configureHttClient(c));

        return services;
    }
}
