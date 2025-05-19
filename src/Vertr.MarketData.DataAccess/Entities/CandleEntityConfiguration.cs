using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Entities;
internal class CandleEntityConfiguration : IEntityTypeConfiguration<Candle>
{
    public void Configure(EntityTypeBuilder<Candle> builder)
    {
        builder.ToTable("candles");

        builder.HasKey(e => e.Id)
            .HasName("candles_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.TimeUtc)
            .HasColumnName("time_utc")
            .IsRequired();

        builder.Property(e => e.Symbol)
            .HasColumnName("symbol")
            .IsRequired();

        builder.Property(e => e.Interval)
            .HasColumnName("interval")
            .IsRequired();

        builder.Property(e => e.Source)
            .HasColumnName("source")
            .IsRequired();

        builder.Property(e => e.Open)
            .HasColumnName("open");

        builder.Property(e => e.Close)
            .HasColumnName("close");

        builder.Property(e => e.High)
            .HasColumnName("high");

        builder.Property(e => e.Low)
            .HasColumnName("low");

        builder.Property(e => e.Volume)
            .HasColumnName("volume");

        builder
            .HasIndex(e => new
            {
                e.TimeUtc,
                e.Interval,
                e.Source,
                e.Symbol,
            })
            .IsUnique()
            .HasDatabaseName("candles_unique");
    }

}
}
