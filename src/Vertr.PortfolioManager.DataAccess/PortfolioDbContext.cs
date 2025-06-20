using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Application.Entities;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.DataAccess.Entities;

namespace Vertr.PortfolioManager.DataAccess;

public class PortfolioDbContext : DbContext
{
    public DbSet<PortfolioPosition> Positions { get; set; }

    public DbSet<PortfolioSnapshot> Portfolios { get; set; }

    public DbSet<OperationEvent> Operations { get; set; }

    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new PortfolioSnapshotEntityConfiguration().Configure(modelBuilder.Entity<PortfolioSnapshot>());

        new PortfolioPositionEntityConfiguration().Configure(modelBuilder.Entity<PortfolioPosition>());

        new OperationEventEntityConfiguration().Configure(modelBuilder.Entity<OperationEvent>());
    }
}
