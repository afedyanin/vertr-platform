using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Vertr.PortfolioManager.Contracts;

public static class PortfolioRegistrar
{
    public static IServiceCollection AddortfolioClient(
        this IServiceCollection services,
        Action<HttpClient> configureHttClient)
    {
        services
              .AddRefitClient<IPortfolioClient>()
              .ConfigureHttpClient(c => configureHttClient(c));

        return services;
    }
}
