using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess.Entities;
internal class PostOrderResponseConfiguration : IEntityTypeConfiguration<PostOrderResponse>
{
    public void Configure(EntityTypeBuilder<PostOrderResponse> builder)
    {
        builder.ToTable("tinvest_orders");

        builder.HasKey(e => e.Id)
            .HasName("tinvest_orders_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.TimeUtc)
            .HasColumnName("time_utc")
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(e => e.TradingSignalId)
            .HasColumnName("trading_signal_id");

        builder.Property(e => e.OrderId)
            .HasColumnName("order_id");

        builder.Property(e => e.OrderRequestId)
            .HasColumnName("order_request_id");

        builder.Property(e => e.ExecutionReportStatus)
            .HasColumnName("execution_report_status");

        builder.Property(e => e.LotsRequested)
            .HasColumnName("lots_requested");

        builder.Property(e => e.LotsExecuted)
            .HasColumnName("lots_executed");

        builder.Property(e => e.InitialOrderPrice)
            .HasColumnName("initial_order_price");

        builder.Property(e => e.ExecutedOrderPrice)
            .HasColumnName("executed_order_price");

        builder.Property(e => e.TotalOrderAmount)
            .HasColumnName("total_order_amount");

        builder.Property(e => e.InitialCommission)
            .HasColumnName("initial_commission");

        builder.Property(e => e.ExecutedCommission)
            .HasColumnName("executed_commission");

        builder.Property(e => e.Direction)
            .HasColumnName("direction");

        builder.Property(e => e.InitialSecurityPrice)
            .HasColumnName("initial_security_price");

        builder.Property(e => e.OrderType)
            .HasColumnName("order_type");

        builder.Property(e => e.Message)
            .HasColumnName("message");

        builder.Property(e => e.InstrumentUid)
            .HasColumnName("instrument_uid");
    }
}
