using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.DataAccess;
public static class DataAccessRegistrar
{
    public static readonly string ConnectionStringName = "VertrDbConnection";
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        var connectionString = configuration.GetConnectionString(ConnectionStringName);
        services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));
        services.AddScoped<ITinvestCandlesRepository, TinvestCandlesRepository>();

        return services;
    }
}
