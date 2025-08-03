using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Pgsql;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.DataAccess.Repositories;

namespace Vertr.MarketData.DataAccess;
public static class DataAccessRegistrar
{
    public static IServiceCollection AddMarketDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<MarketDataDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IMarketDataInstrumentRepository, InstrumentsRepository>();
        services.AddScoped<ICandlesRepository, CandlesRepository>();

        return services;
    }
}
