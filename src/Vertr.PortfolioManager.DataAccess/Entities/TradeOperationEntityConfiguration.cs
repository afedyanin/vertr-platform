using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Entities;

internal class TradeOperationEntityConfiguration : IEntityTypeConfiguration<TradeOperation>
{
    public void Configure(EntityTypeBuilder<TradeOperation> builder)
    {
        builder.ToTable("operations");

        builder.HasKey(e => e.Id)
            .HasName("operations_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.OrderId)
            .HasColumnName("order_id");

        builder.Property(e => e.OperationType)
            .HasColumnName("operation_type");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.SubAccountId)
            .HasColumnName("sub_account_id")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
            .IsRequired();

        builder.Property(e => e.Amount)
            .HasColumnName("amount");

        builder.Property(e => e.TradeId)
            .HasColumnName("trade_id");

        builder.Property(e => e.ExecutionTime)
            .HasColumnName("execution_time");

        builder.Property(e => e.Price)
            .HasColumnName("price");

        builder.Property(e => e.Quantity)
            .HasColumnName("quantity");

        builder.Property(e => e.Message)
            .HasColumnName("message");
    }
}

