using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess.Entities;
internal class OperationEntityConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        builder.ToTable("tinvest_operations");

        builder.HasKey(e => e.Id)
            .HasName("tinvest_operations_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.ParentOperationId)
            .HasColumnName("parent_operation_id");

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.Currency)
            .HasColumnName("currency")
            .IsRequired();

        builder.Property(e => e.Payment)
            .HasColumnName("payment");

        builder.Property(e => e.Price)
            .HasColumnName("price");

        builder.Property(e => e.State)
            .HasColumnName("state");

        builder.Property(e => e.Quantity)
            .HasColumnName("quantity");

        builder.Property(e => e.QuantityRest)
            .HasColumnName("quantity_rest");

        builder.Property(e => e.InstrumentType)
            .HasColumnName("instrument_type");

        builder.Property(e => e.Date)
            .HasColumnName("date")
            .IsRequired();

        builder.Property(e => e.Type)
            .HasColumnName("type");

        builder.Property(e => e.OperationType)
            .HasColumnName("operation_type");

        builder.Property(e => e.AssetUid)
            .HasColumnName("asset_uid");

        builder.Property(e => e.PositionUid)
            .HasColumnName("position_uid");

        builder.Property(e => e.InstrumentUid)
            .HasColumnName("instrument_uid");

        builder.Ignore(e => e.OperationTrades);
    }
}
