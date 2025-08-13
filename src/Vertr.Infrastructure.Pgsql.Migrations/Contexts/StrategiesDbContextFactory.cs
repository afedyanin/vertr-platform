using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vertr.Strategies.DataAccess;

namespace Vertr.Infrastructure.Pgsql.Migrations.Contexts;
internal class StrategiesDbContextFactory : IDesignTimeDbContextFactory<StrategiesDbContext>
{
    public StrategiesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StrategiesDbContext>();

        optionsBuilder.UseNpgsql(
            ConnectionStrings.LocalConnection, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new StrategiesDbContext(optionsBuilder.Options);
    }
}
