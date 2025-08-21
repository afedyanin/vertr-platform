
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.DataAccess.Entities;

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

        builder.Property(e => e.StrategyId)
            .HasColumnName("strategy_id")
            .IsRequired();

        builder.Property(e => e.PortfolioId)
            .HasColumnName("sub_account_id")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
            .IsRequired();

        builder.Property(e => e.BacktestId)
            .HasColumnName("backtest_id");

        builder.Property(e => e.QtyLots)
            .HasColumnName("qty_lots")
            .IsRequired();

        builder.Property(e => e.Price)
            .HasColumnName("price");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}
