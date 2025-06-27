using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Entities;

internal class TradeOperationEntityConfiguration : IEntityTypeConfiguration<TradeOperation>
{
    public void Configure(EntityTypeBuilder<TradeOperation> builder)
    {
        builder.ToTable("operation_events");

        builder.HasKey(e => e.Id)
            .HasName("operation_events_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.BookId)
            .HasColumnName("book_id");

        builder.Property(e => e.ClassCode)
            .HasColumnName("class_code")
            .IsRequired();

        builder.Property(e => e.ClassCode)
            .HasColumnName("ticker")
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

