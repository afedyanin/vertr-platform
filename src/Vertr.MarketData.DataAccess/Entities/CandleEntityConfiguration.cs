using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Entities;

internal class CandleEntityConfiguration : IEntityTypeConfiguration<Candle>
{
    public static readonly string CandlesTableName = "candles";

    public void Configure(EntityTypeBuilder<Candle> builder)
    {
        builder.ToTable(CandlesTableName);

        builder.HasNoKey();

        builder.Property(e => e.TimeUtc)
            .HasColumnName("time_utc")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
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
                e.InstrumentId,
            })
            .IsUnique()
            .HasDatabaseName("candles_unique");
    }
}
