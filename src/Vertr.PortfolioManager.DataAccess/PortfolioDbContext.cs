using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.DataAccess.Entities;

namespace Vertr.PortfolioManager.DataAccess;

public class PortfolioDbContext : DbContext
{
    public DbSet<TradeOperation> Operations { get; set; }

    public DbSet<Portfolio> Portfolios { get; set; }

    public DbSet<Position> Positions { get; set; }

    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new TradeOperationEntityConfiguration().Configure(modelBuilder.Entity<TradeOperation>());
        new PortfolioEntityConfiguration().Configure(modelBuilder.Entity<Portfolio>());
        new PositionEntityConfiguration().Configure(modelBuilder.Entity<Position>());
    }
}
