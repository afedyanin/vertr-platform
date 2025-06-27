using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.DataAccess.Entities;

namespace Vertr.PortfolioManager.DataAccess;

public class PortfolioDbContext : DbContext
{
    public DbSet<TradeOperation> Operations { get; set; }

    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new TradeOperationEntityConfiguration().Configure(modelBuilder.Entity<TradeOperation>());
    }
}
