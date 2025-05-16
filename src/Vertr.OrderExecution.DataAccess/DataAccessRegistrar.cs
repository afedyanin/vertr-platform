using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Infrastructure.Pgsql;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.DataAccess.Repositories;

namespace Vertr.OrderExecution.DataAccess;
public static class DataAccessRegistrar
{
    public static readonly string ConnectionStringName = "VertrDbConnection";
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString!));
        services.AddDbContextFactory<OrderExecutionDbContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton<IOrderEventRepository, OrderEventRepository>();

        return services;
    }
}
