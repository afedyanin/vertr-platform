using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.DataAccess.Entities;

namespace Vertr.PortfolioManager.DataAccess;

public class PortfolioDbContext : DbContext
{
    public DbSet<OperationEvent> Operations { get; set; }

    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new OperationEventEntityConfiguration().Configure(modelBuilder.Entity<OperationEvent>());
    }
}
