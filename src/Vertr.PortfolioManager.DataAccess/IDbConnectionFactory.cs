using System.Data;

namespace Vertr.Adapters.DataAccess;

internal interface IDbConnectionFactory
{
    IDbConnection GetConnection();
}
