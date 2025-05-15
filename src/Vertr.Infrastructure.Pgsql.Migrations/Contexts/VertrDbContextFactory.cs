using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vertr.Adapters.DataAccess;

namespace Vertr.Infrastructure.Pgsql.Migrations.Contexts;

public class VertrDbContextFactory : IDesignTimeDbContextFactory<VertrDbContext>
{
    public VertrDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VertrDbContext>();

        optionsBuilder.UseNpgsql(
            ConnectionStrings.LocalConnection, b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new VertrDbContext(optionsBuilder.Options);
    }
}
