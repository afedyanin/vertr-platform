using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Pgsql;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.DataAccess.Mappers;
using Vertr.MarketData.DataAccess.Repositories;

namespace Vertr.MarketData.DataAccess;
public static class DataAccessRegistrar
{
    public static IServiceCollection AddMarketDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<MarketDataDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IInstrumentsRepository, InstrumentsRepository>();
        services.AddScoped<ICandlesRepository, CandlesRepository>();
        services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
        services.AddScoped<ICandlesHistoryRepository, CandlesHistoryRepository>();

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        return services;
    }
}
