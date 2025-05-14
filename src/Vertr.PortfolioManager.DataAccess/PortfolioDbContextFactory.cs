using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Vertr.Adapters.DataAccess;

public class PortfolioDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
{
    public PortfolioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PortfolioDbContext>();

        optionsBuilder.UseNpgsql<PortfolioDbContext>("Server=localhost;Port=5432;User Id=postgres;Password=admin;Database=vertr;");

        return new PortfolioDbContext(optionsBuilder.Options);
    }
}
