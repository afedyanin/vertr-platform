using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Entities;

internal class PositionEntityConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(e => e.Id)
            .HasName("positions_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.PortfolioId)
            .HasColumnName("portfolio_id")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
            .IsRequired();

        builder.Property(e => e.Balance)
            .HasColumnName("balance")
            .IsRequired();

        builder
            .HasIndex(e => new
            {
                e.PortfolioId,
                e.InstrumentId,
            })
            .IsUnique()
            .HasDatabaseName("positions_unique");
    }
}
