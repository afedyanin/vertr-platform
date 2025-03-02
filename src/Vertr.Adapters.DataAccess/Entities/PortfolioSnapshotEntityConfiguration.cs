using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess.Entities;
internal class PortfolioSnapshotEntityConfiguration : IEntityTypeConfiguration<PortfolioSnapshot>
{
    public void Configure(EntityTypeBuilder<PortfolioSnapshot> builder)
    {
        builder.ToTable("tinvest_portfolio_snapshots");

        builder.HasKey(e => e.Id)
            .HasName("tinvest_portfolio_snapshots_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.TimeUtc)
            .HasColumnName("time_utc")
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.TotalAmountShares)
            .HasColumnName("total_amount_shares");

        builder.Property(e => e.TotalAmountBonds)
            .HasColumnName("total_amount_bonds");

        builder.Property(e => e.TotalAmountEtf)
            .HasColumnName("total_amount_etf");

        builder.Property(e => e.TotalAmountCurrencies)
            .HasColumnName("total_amount_currencies");

        builder.Property(e => e.TotalAmountFutures)
            .HasColumnName("total_amount_futures");

        builder.Property(e => e.TotalAmountOptions)
            .HasColumnName("total_amount_options");

        builder.Property(e => e.TotalAmountSp)
            .HasColumnName("total_amount_sp");

        builder.Property(e => e.TotalAmountPortfolio)
            .HasColumnName("total_amount_portfolio");

        builder.Property(e => e.ExpectedYield)
            .HasColumnName("expected_yield");

        builder.HasMany(e => e.Positions)
            .WithOne(p => p.PortfolioSnapshot)
            .HasForeignKey(e => e.PortfolioSnapshotId)
            .HasConstraintName("tinvest_portfolio_position_snapshot_fk");
    }
}
