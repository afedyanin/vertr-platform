using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Entities;
internal class PortfolioPositionEntityConfiguration : IEntityTypeConfiguration<PortfolioPosition>
{
    public void Configure(EntityTypeBuilder<PortfolioPosition> builder)
    {
        builder.ToTable("portfolio_positions");

        builder.HasKey(e => e.Id)
            .HasName("portfolio_positions_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.PortfolioSnapshotId)
            .HasColumnName("portfolio_snapshot_id")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id");

        builder.Property(e => e.Balance)
            .HasColumnName("balance");
    }
}
