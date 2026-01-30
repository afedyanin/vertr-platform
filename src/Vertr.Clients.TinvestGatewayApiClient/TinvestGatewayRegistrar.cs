using Microsoft.Extensions.DependencyInjection;
using Refit;
using Vertr.Common.Application.Abstractions;
using Vertr.Common.Contracts;

namespace Vertr.Clients.TinvestGatewayApiClient;

public static class TinvestGatewayRegistrar
{
    public static IServiceCollection AddTinvestGateway(this IServiceCollection services, string baseAddress)
    {
        services.AddSingleton<ITradingGateway, TinvestGateway>();

        services
           .AddRefitClient<ITinvestGatewayClient>(
               new RefitSettings
               {
                   ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions.DefaultOptions)
               })
           .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));

        return services;
    }
}
