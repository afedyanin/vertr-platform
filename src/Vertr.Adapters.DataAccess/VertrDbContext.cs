using Microsoft.EntityFrameworkCore;
using Vertr.Adapters.DataAccess.Entities;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess;

public class VertrDbContext : DbContext
{
    public DbSet<Operation> TinvestOperations { get; set; }

    public DbSet<PortfolioSnapshot> TinvestPortfolios { get; set; }

    public DbSet<PostOrderResponse> TinvestOrders { get; set; }

    public DbSet<TradingSignal> TradingSignals { get; set; }

    public DbSet<HistoricCandle> TinvestCandles { get; set; }

    public VertrDbContext(DbContextOptions<VertrDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new OperationEntityConfiguration().Configure(modelBuilder.Entity<Operation>());

        new PortfolioSnapshotEntityConfiguration().Configure(modelBuilder.Entity<PortfolioSnapshot>());

        new PortfolioPositionEntityConfiguration().Configure(modelBuilder.Entity<PortfolioPosition>());

        new PostOrderResponseConfiguration().Configure(modelBuilder.Entity<PostOrderResponse>());

        new TradingSignalEntityConfiguration().Configure(modelBuilder.Entity<TradingSignal>());

        new HistoricCandleEntityConfiguration().Configure(modelBuilder.Entity<HistoricCandle>());
    }
}
