using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Pgsql;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.DataAccess.Repositories;

namespace Vertr.PortfolioManager.DataAccess;
public static class DataAccessRegistrar
{
    public static IServiceCollection AddPortfolioManagerDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<PortfolioDbContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton<IOperationEventRepository, OperationEventRepository>();

        return services;
    }
}
