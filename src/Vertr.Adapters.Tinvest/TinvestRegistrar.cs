using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinkoff.InvestApi;
using Vertr.Adapters.Tinvest.Converters;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Tinvest;

public static class TinvestRegistrar
{
    public static IServiceCollection AddTinvestGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddOptions<TinvestSettings>().BindConfiguration(nameof(TinvestSettings));
        services.AddInvestApiClient((_, settings) => configuration.Bind($"{nameof(TinvestSettings)}:{nameof(InvestApiSettings)}", settings));
        services.AddScoped<ITinvestGateway, TinvestGateway>();
        services.AddAutoMapper(typeof(TinvestMappingProfile));
        return services;
    }
}
