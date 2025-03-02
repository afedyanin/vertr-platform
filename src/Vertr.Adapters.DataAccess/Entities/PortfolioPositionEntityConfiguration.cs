using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess.Entities;

internal class PortfolioPositionEntityConfiguration : IEntityTypeConfiguration<PortfolioPosition>
{
    public void Configure(EntityTypeBuilder<PortfolioPosition> builder)
    {
        builder.ToTable("tinvest_portfolio_positions");

        builder.HasKey(e => e.Id)
            .HasName("tinvest_portfolio_positions_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.PortfolioSnapshotId)
            .HasColumnName("portfolio_snapshot_id")
            .IsRequired();

        builder.Property(e => e.InstrumentType)
            .HasColumnName("instrument_type");

        builder.Property(e => e.Quantity)
            .HasColumnName("quantity");

        builder.Property(e => e.AveragePositionPrice)
            .HasColumnName("average_position_price");

        builder.Property(e => e.ExpectedYield)
            .HasColumnName("expected_yield");

        builder.Property(e => e.CurrentNkd)
            .HasColumnName("current_nkd");

        builder.Property(e => e.CurrentPrice)
            .HasColumnName("current_price");

        builder.Property(e => e.AveragePositionPriceFifo)
            .HasColumnName("average_position_price_fifo");

        builder.Property(e => e.Blocked)
            .HasColumnName("blocked");

        builder.Property(e => e.BlockedLots)
            .HasColumnName("blocked_lots");

        builder.Property(e => e.PositionUid)
            .HasColumnName("position_uid");

        builder.Property(e => e.InstrumentUid)
            .HasColumnName("instrument_uid");

        builder.Property(e => e.VarMargin)
            .HasColumnName("var_margin");

        builder.Property(e => e.ExpectedYieldFifo)
            .HasColumnName("expected_yield_fifo");
    }
}
