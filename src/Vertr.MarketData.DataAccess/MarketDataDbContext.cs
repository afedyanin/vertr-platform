using Microsoft.EntityFrameworkCore;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.DataAccess.Entities;

namespace Vertr.MarketData.DataAccess;

public class MarketDataDbContext : DbContext
{
    public DbSet<Instrument> Instruments { get; set; }

    public DbSet<Candle> Candles { get; set; }

    public MarketDataDbContext(DbContextOptions<MarketDataDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new InstrumentEntityConfiguration().Configure(modelBuilder.Entity<Instrument>());
        new CandleEntityConfiguration().Configure(modelBuilder.Entity<Candle>());
    }
}
