using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess.Entities;
internal class HistoricCandleEntityConfiguration : IEntityTypeConfiguration<HistoricCandle>
{
    public static readonly string TinvestCandlesTableName = "tinvest_candles";

    public void Configure(EntityTypeBuilder<HistoricCandle> builder)
    {
        builder.ToTable(TinvestCandlesTableName);

        builder.HasNoKey();

        builder.Property(e => e.TimeUtc)
            .HasColumnName("time_utc")
            .IsRequired();

        builder.Property(e => e.Interval)
            .HasColumnName("interval")
            .IsRequired();

        builder.Property(e => e.Symbol)
            .HasColumnName("symbol")
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

        builder.Property(e => e.IsCompleted)
            .HasColumnName("is_completed");

        builder.Property(e => e.CandleSource)
            .HasColumnName("candle_source")
            .IsRequired();

        builder
            .HasIndex(e => new
            {
                e.TimeUtc,
                e.Interval,
                e.Symbol,
            })
            .IsUnique()
            .HasDatabaseName("tinvest_candles_unique");
    }
}
