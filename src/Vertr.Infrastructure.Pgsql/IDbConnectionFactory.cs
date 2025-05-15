using System.Data;

namespace Vertr.Infrastructure.Pgsql;

public interface IDbConnectionFactory
{
    IDbConnection GetConnection();
}
