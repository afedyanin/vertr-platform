using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Pgsql;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.DataAccess.Repositories;

namespace Vertr.OrderExecution.DataAccess;

public static class DataAccessRegistrar
{
    public static IServiceCollection AddOrderExecutionDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<OrderExecutionDbContext>(options => options.UseNpgsql(connectionString));
        services.AddSingleton<IOrderEventRepository, OrderEventRepository>();

        return services;
    }
}
