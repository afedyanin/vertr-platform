using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Entities;
internal class CandleHistoryEntityConfiguration : IEntityTypeConfiguration<CandlesHistoryItem>
{
    public static readonly string CandlesHistoryTableName = "candles_history";

    public void Configure(EntityTypeBuilder<CandlesHistoryItem> builder)
    {
        builder.ToTable(CandlesHistoryTableName);

        builder.HasKey(e => e.Id)
            .HasName("candles_history_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
            .IsRequired();

        builder.Property(e => e.Interval)
            .HasColumnName("interval")
            .IsRequired();

        builder.Property(e => e.Day)
            .HasColumnName("day")
            .IsRequired();

        builder.Property(e => e.Data)
            .HasColumnName("data")
            .IsRequired();

        builder.Property(e => e.Count)
            .HasColumnName("count")
            .IsRequired();

        builder
            .HasIndex(e => new
            {
                e.InstrumentId,
                e.Interval,
                e.Day,
            })
            .IsUnique()
            .HasDatabaseName("candles_history_unique");
    }
}
