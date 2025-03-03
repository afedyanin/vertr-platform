using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess.Entities;

internal class TradingSignalEntityConfiguration : IEntityTypeConfiguration<TradingSignal>
{
    public void Configure(EntityTypeBuilder<TradingSignal> builder)
    {
        builder.ToTable("trading_signals");

        builder.HasKey(e => e.Id)
            .HasName("trading_signals_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.TimeUtc)
            .HasColumnName("time_utc")
            .IsRequired();

        builder.Property(e => e.Symbol)
            .HasColumnName("symbol")
            .IsRequired();

        builder.Property(e => e.Action)
            .HasColumnName("action")
            .IsRequired();

        builder.Property(e => e.CandleInterval)
            .HasColumnName("candle_interval")
            .IsRequired();

        builder.Property(e => e.PredictorType)
            .HasColumnName("predictor_type")
            .IsRequired();

        builder.Property(e => e.Sb3Algo)
            .HasColumnName("sb3_algo");

        builder.Property(e => e.CandlesSource)
            .HasColumnName("candles_source");

        builder.Property(e => e.QuantityLots)
            .HasColumnName("quantity_lots");
    }
}
