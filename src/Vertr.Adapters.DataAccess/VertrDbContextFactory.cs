using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Vertr.Adapters.DataAccess;

public class VertrDbContextFactory : IDesignTimeDbContextFactory<VertrDbContext>
{
    public VertrDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VertrDbContext>();

        optionsBuilder.UseNpgsql<VertrDbContext>("Server=localhost;Port=5432;User Id=postgres;Password=admin;Database=vertr;");

        return new VertrDbContext(optionsBuilder.Options);
    }
}
