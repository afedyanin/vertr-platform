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

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.OperationType)
            .HasColumnName("operation_type")
            .IsRequired();

        builder.Property(e => e.OrderId)
            .HasColumnName("order_id");

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.SubAccountId)
            .HasColumnName("sub_account_id")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
            .IsRequired();

        var amount = builder.ComplexProperty(e => e.Amount);
        amount.Property(e => e.Value).HasColumnName("amount_value");
        amount.Property(e => e.Currency).HasColumnName("amount_currency");

        builder.Property(e => e.TradeId)
            .HasColumnName("trade_id");

        var price = builder.ComplexProperty(e => e.Price);
        price.Property(e => e.Value).HasColumnName("price_value");
        price.Property(e => e.Currency).HasColumnName("price_currency");

        builder.Property(e => e.Quantity)
            .HasColumnName("quantity");
    }
}

