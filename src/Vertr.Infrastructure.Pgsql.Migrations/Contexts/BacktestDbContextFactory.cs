using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vertr.Backtest.DataAccess;

namespace Vertr.Infrastructure.Pgsql.Migrations.Contexts;

internal class BacktestDbContextFactory : IDesignTimeDbContextFactory<BacktestDbContext>
{
    public BacktestDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BacktestDbContext>();

        optionsBuilder.UseNpgsql(
            ConnectionStrings.LocalConnection, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new BacktestDbContext(optionsBuilder.Options);
    }
}
