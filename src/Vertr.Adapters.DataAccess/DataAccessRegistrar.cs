using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Adapters.DataAccess.Repositories;
using Vertr.Domain.Repositories;
using Vertr.Infrastructure.Pgsql;

namespace Vertr.Adapters.DataAccess;
public static class DataAccessRegistrar
{
    public static readonly string ConnectionStringName = "VertrDbConnection";
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddScoped<ITinvestCandlesRepository, TinvestCandlesRepository>();
        services.AddScoped<ITradingSignalsRepository, TradingSignalsRepository>();

        services.AddDbContextFactory<VertrDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ITinvestPortfolioRepository, TinvestPortfolioRepository>();
        services.AddScoped<ITinvestOperationsRepository, TinvestOperationsRepository>();
        services.AddScoped<ITinvestOrdersRepository, TinvestOrdersRepository>();

        return services;
    }
}
