using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Vertr.MarketData.DataAccess;
using System.Reflection;

namespace Vertr.Infrastructure.Pgsql.Migrations.Contexts;

public class MarketDataDbContextFactory : IDesignTimeDbContextFactory<MarketDataDbContext>
{
    public MarketDataDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MarketDataDbContext>();

        optionsBuilder.UseNpgsql(
            ConnectionStrings.LocalConnection, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new MarketDataDbContext(optionsBuilder.Options);
    }
}


