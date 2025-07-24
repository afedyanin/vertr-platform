using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.DataAccess.Entities;

internal class StrategyEntityConfiguration : IEntityTypeConfiguration<StrategyMetadata>
{
    public void Configure(EntityTypeBuilder<StrategyMetadata> builder)
    {
        builder.ToTable("strategies");

        builder.HasKey(e => e.Id)
            .HasName("strategies_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("description");

        builder.Property(e => e.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.SubAccountId)
            .HasColumnName("sub_account_id")
            .IsRequired();

        builder.Property(e => e.QtyLots)
            .HasColumnName("qty_lots")
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(e => e.ParamsJson)
            .HasColumnName("params_json")
            .HasColumnType("json");

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
