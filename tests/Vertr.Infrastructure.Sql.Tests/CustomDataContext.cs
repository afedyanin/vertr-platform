using Dapper;
using Microsoft.Data.Sqlite;

namespace Vertr.Infrastructure.Sql.Tests;
internal class CustomDataContext
{
    private const string Sql = @"CREATE TABLE IF NOT EXISTS
            Author
            (
                Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                FirstName       TEXT NOT NULL,
                LastName        TEXT NOT NULL,
                Address         TEXT NOT NULL
            )";

    internal SqliteConnection CreateDatabaseConnection(string databaseName)
    {
        return new SqliteConnection("Data Source=" + databaseName);
    }

    public async Task<SqliteConnection> CreateDatabase(string databaseName)
    {
        using var sqliteConnection = CreateDatabaseConnection(databaseName);

        _ = await sqliteConnection.ExecuteAsync(Sql);

        return sqliteConnection;
    }
}
