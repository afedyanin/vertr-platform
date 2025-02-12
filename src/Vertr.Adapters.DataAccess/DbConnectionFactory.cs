using System.Data;
using Npgsql;

namespace Vertr.Adapters.DataAccess;
internal class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(nameof(connectionString));
        _connectionString = connectionString;
    }

    public IDbConnection GetConnection() => new NpgsqlConnection(_connectionString);
}
