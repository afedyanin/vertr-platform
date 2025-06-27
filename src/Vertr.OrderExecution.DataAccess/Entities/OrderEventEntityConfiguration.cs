using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.DataAccess.Entities;

internal class OrderEventEntityConfiguration : IEntityTypeConfiguration<OrderEvent>
{
    public void Configure(EntityTypeBuilder<OrderEvent> builder)
    {
        builder.ToTable("order_events");

        builder.HasKey(e => e.Id)
            .HasName("order_events_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.RequestId)
            .HasColumnName("request_id");

        builder.Property(e => e.OrderId)
            .HasColumnName("order_id");

        builder.Property(e => e.ClassCode)
            .HasColumnName("class_code")
            .IsRequired();

        builder.Property(e => e.Ticker)
            .HasColumnName("ticker")
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.BookId)
            .HasColumnName("book_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.JsonDataType)
            .HasColumnName("json_data_type");

        builder.Property(e => e.JsonData)
            .HasColumnName("json_data")
            .HasColumnType("json");
    }
}
