using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Entities;

internal class PortfolioEntityConfiguration : IEntityTypeConfiguration<Portfolio>
{
    public static readonly string PortfoliosTableName = "portfolios";

    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.ToTable(PortfoliosTableName);

        builder.HasKey(e => e.Id)
            .HasName("portfolios_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName("name");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(e => e.IsBacktest)
            .HasColumnName("is_backtest");

        builder
            .HasMany(e => e.Positions)
            .WithOne()
            .HasForeignKey(e => e.PortfolioId)
            .IsRequired();
    }
}
