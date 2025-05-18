using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.DataAccess.Entities;

internal class OperationEventEntityConfiguration : IEntityTypeConfiguration<OperationEvent>
{
    public void Configure(EntityTypeBuilder<OperationEvent> builder)
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

        builder.Property(e => e.JsonDataType)
            .HasColumnName("json_data_type");

        builder.Property(e => e.JsonData)
            .HasColumnName("json_data")
            .HasColumnType("json");
    }
}

