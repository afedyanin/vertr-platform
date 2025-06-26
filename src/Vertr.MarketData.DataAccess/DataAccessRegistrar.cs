using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.DataAccess.Repositories;

namespace Vertr.MarketData.DataAccess;

public static class DataAccessRegistrar
{
    public static IServiceCollection AddMarketDataDataAccess(this IServiceCollection services)
    {
        services.AddTransient<IMarketInstrumentRepository, MarketInstrumentRepository>();

        return services;
    }
}
