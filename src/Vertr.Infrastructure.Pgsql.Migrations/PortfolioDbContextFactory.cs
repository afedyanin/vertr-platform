using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Vertr.Infrastructure.Pgsql.Migrations;
using System.Reflection;

namespace Vertr.PortfolioManager.DataAccess;

public class PortfolioDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
{
    public PortfolioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PortfolioDbContext>();

        optionsBuilder.UseNpgsql(
            ConnectionStrings.LocalConnection, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new PortfolioDbContext(optionsBuilder.Options);
    }
}
