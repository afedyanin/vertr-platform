using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vertr.MarketData.DataAccess;

namespace Vertr.Infrastructure.Pgsql.Migrations.Contexts;

internal class MarketDataDbContextFactory : IDesignTimeDbContextFactory<MarketDataDbContext>
{
    public MarketDataDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MarketDataDbContext>();

        optionsBuilder.UseNpgsql(
            ConnectionStrings.LocalConnection, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new MarketDataDbContext(optionsBuilder.Options);
    }

}
