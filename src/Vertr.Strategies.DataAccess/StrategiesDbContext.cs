using Microsoft.EntityFrameworkCore;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.DataAccess.Entities;

namespace Vertr.Strategies.DataAccess;

public class StrategiesDbContext : DbContext
{
    public DbSet<StrategyMetadata> Strategies { get; set; }
    public DbSet<TradingSignal> TradingSignals { get; set; }

    public StrategiesDbContext(DbContextOptions<StrategiesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new StrategyEntityConfiguration().Configure(modelBuilder.Entity<StrategyMetadata>());
        new TradingSignalEntityConfiguration().Configure(modelBuilder.Entity<TradingSignal>());
    }

}
