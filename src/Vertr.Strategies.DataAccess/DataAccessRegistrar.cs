using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Pgsql;
using Vertr.Strategies.Contracts.Interfaces;
using Vertr.Strategies.DataAccess.Repositories;

namespace Vertr.Strategies.DataAccess;
public static class DataAccessRegistrar
{
    public static IServiceCollection AddStrategiesDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<StrategiesDbContext>(options => options.UseNpgsql(connectionString));

        services.AddTransient<IStrategyMetadataRepository, StrategyMetadataRepository>();
        services.AddTransient<ITradingSignalRepository, TradingSignalRepository>();

        return services;
    }
}
