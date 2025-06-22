using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Entities;
internal class PortfolioSnapshotEntityConfiguration : IEntityTypeConfiguration<PortfolioSnapshot>
{
    public void Configure(EntityTypeBuilder<PortfolioSnapshot> builder)
    {
        builder.ToTable("portfolio_snapshots");

        builder.HasKey(e => e.Id)
            .HasName("portfolio_snapshots_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.BookId)
            .HasColumnName("book_id");

        builder.Property(e => e.JsonDataType)
            .HasColumnName("json_data_type");

        builder.Property(e => e.JsonData)
            .HasColumnName("json_data")
            .HasColumnType("json");

        // TODO Fix it
        //builder.HasMany(e => e.Positions)
            //.WithOne(p => p.PortfolioSnapshot)
            //.HasForeignKey(e => e.PortfolioSnapshotId)
            //.HasConstraintName("portfolio_position_snapshot_fk");
    }
}
