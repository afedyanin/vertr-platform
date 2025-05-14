using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.DataAccess.Repositories;

namespace Vertr.PortfolioManager.DataAccess;
public static class DataAccessRegistrar
{
    public static readonly string ConnectionStringName = "VertrDbConnection";
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<PortfolioDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IPortfolioSnapshotRepository, PortfolioSnapshotRepository>();

        return services;
    }
}
