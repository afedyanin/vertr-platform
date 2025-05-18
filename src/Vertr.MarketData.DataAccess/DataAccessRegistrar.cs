using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Pgsql;
using Vertr.MarketData.Application.Abstractions;
using Vertr.MarketData.DataAccess.Repositories;

namespace Vertr.MarketData.DataAccess;
public static class DataAccessRegistrar
{
    public static readonly string ConnectionStringName = "VertrDbConnection";
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<MarketDataDbContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton<ICandlesRepository, CandlesRepository>();

        return services;
    }
}
