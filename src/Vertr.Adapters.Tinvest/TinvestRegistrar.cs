using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Tinvest;

public static class TinvestRegistrar
{
    public static IServiceCollection AddTinvestGateway(this IServiceCollection services)
    {
        // TODO: refactor this
        var config = new TinvestConfiguration();
        services.AddInvestApiClient((_, settings) => settings.AccessToken = config.Token);
        services.AddScoped<ITinvestGateway, TinvestGateway>();

        return services;
    }
}
