using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Backtest.DataAccess.Repositories;
using Vertr.Infrastructure.Pgsql;

namespace Vertr.Backtest.DataAccess;

public static class DataAccessRegistrar
{
    public static IServiceCollection AddBacktestDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<BacktestDbContext>(options => options.UseNpgsql(connectionString));
        services.AddTransient<IBacktestRepository, BacktestRepository>();

        return services;
    }
}
