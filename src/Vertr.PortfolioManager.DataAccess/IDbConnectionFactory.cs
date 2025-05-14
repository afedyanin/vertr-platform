using System.Data;

namespace Vertr.PortfolioManager.DataAccess;

internal interface IDbConnectionFactory
{
    IDbConnection GetConnection();
}
